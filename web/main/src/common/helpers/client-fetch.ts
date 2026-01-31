import { type ActionType } from '@models/actions.js';
import type { ErrorResponse } from '@models/error-response';
import { isSingleFieldAction } from './error-helper';
import { NotificationType } from '@models/notification.types';
import { notificationMessages } from '@lib/stores/notification-message';
import { id } from 'zod/v4/locales';
import { activeAccount, defaultScopes } from '@lib/stores/auth';
import { get } from 'svelte/store';
import { msalInstanceStore } from '@lib/stores/auth';
import { configStore } from '@lib/stores/config-store';

type ClientFetchValue =
	| string
	| number
	| boolean
	| File
	| Blob
	| null
	| undefined
	| ClientFetchValue[]
	| { [key: string]: ClientFetchValue };

export interface Result<TResult> {
	data?: TResult;
	isSuccess?: boolean;
	fieldErrors?: Record<string, string[]>;
	response?: Response;
}

export const clientFetch = async <TResult>(
	path: string,
	action: ActionType,
	data: Record<string, ClientFetchValue>,
	returnResponse?: boolean
): Promise<Result<TResult>> => {
	try {
		console.log('data');
		console.log(data);
		const account = get(activeAccount);
		const msal = get(msalInstanceStore);
		const config = get(configStore);

		const scopes = [...defaultScopes, config.b2c.apiAccessScope];

		console.log('from clientfetch');
		console.log(account);

		const tokenResponse = await msal.acquireTokenSilent({
			scopes: scopes,
			account: account?.account!
		});

		const hasFiles = containsFile(data);

		let body: BodyInit;
		let headers: HeadersInit | undefined;

		if (hasFiles) {
			const formData = new FormData();

			formData.append('action', action);
			formData.append(
				'activeAccount',
				JSON.stringify({
					account: account?.account,
					token: tokenResponse.accessToken
				})
			);

			for (const [key, value] of Object.entries(data)) {
				appendFormData(formData, `data.${key}`, value);
			}

			body = formData;
			headers = undefined;
		} else {
			body = JSON.stringify({
				action,
				data,
				activeAccount: {
					account: account?.account,
					token: tokenResponse.accessToken
				}
			});

			headers = { 'Content-Type': 'application/json' };
		}

		console.log('body');
		console.log(body);

		const response = await fetch(path, {
			method: 'POST',
			body: body,
			headers: headers
		});

		console.log('clientfetch');
		console.log(response);

		if (!response.ok && isSingleFieldAction(action)) {
			console.error(`Failed to action ${action} with status: ${response.status}`);
			const error = (await response.json()) as ErrorResponse;
			notificationMessages.set([
				{
					type: NotificationType.Error,
					message: error.message,
					id: crypto.randomUUID()
				}
			]);
			return { isSuccess: false };
		}

		if (!response.ok && !isSingleFieldAction(action)) {
			const error = (await response.json()) as ErrorResponse;

			if (error.fieldErrors) {
				console.error(
					`Failed to action ${action} with status: ${response.status} and error: ${error.fieldErrors}`
				);
			}

			notificationMessages.set([
				{
					message: error.message ?? 'Failed to action ' + action,
					type: NotificationType.Error,
					id: crypto.randomUUID()
				}
			]);
			return { isSuccess: false, fieldErrors: error.fieldErrors };
		}

		console.log('getting json');

		if (returnResponse) {
			return {
				response: response,
				isSuccess: true
			};
		}

		const json = await response.json();

		console.log('have json');
		//console.log(json);

		return {
			isSuccess: true,
			data: json
		} as Result<TResult>;
	} catch (error) {
		console.error('Exception in clientFetch', error);
		notificationMessages.set([
			{
				type: NotificationType.Error,
				message: 'Failed to action: ' + action,
				id: crypto.randomUUID()
			}
		]);
		return { isSuccess: false };
	}
};

function containsFile(value: unknown): boolean {
	if (value instanceof File || value instanceof Blob) return true;

	if (Array.isArray(value)) {
		return value.some(containsFile);
	}

	if (typeof value === 'object' && value !== null) {
		return Object.values(value).some(containsFile);
	}

	return false;
}

function appendFormData(formData: FormData, key: string, value: ClientFetchValue) {
	console.log('key');
	console.log(key);
	console.log('value');
	console.log(value);
	if (value === undefined || value === null) return;

	if (value instanceof File || value instanceof Blob) {
		formData.append(key, value);
		return;
	}

	if (Array.isArray(value)) {
		value.forEach((v, i) => {
			appendFormData(formData, `${key}[${i}]`, v);
		});
		return;
	}

	if (typeof value === 'object') {
		for (const [k, v] of Object.entries(value)) {
			appendFormData(formData, `${key}.${k}`, v);
		}
		return;
	}

	formData.append(key, String(value));
}
