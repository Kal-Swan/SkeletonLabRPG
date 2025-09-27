import { type ActionType } from '@models/actions.js';
import type { ErrorResponse } from '@models/error-response';
import { isSingleFieldAction } from './error-helper';
import { NotificationType } from '@models/notification.types';
import { notificationMessages } from '@lib/stores/notification-message';
import { id } from 'zod/v4/locales';
import { activeAccount, msalInstance, scopes } from '@lib/auth/msal-client';
import { get } from 'svelte/store';

export interface Result<TResult> {
	data?: TResult;
	isSuccess?: boolean;
	fieldErrors?: Record<string, string[]>;
}

export const clientFetch = async <TResult>(
	path: string,
	action: ActionType,
	data: Record<string, any>
): Promise<Result<TResult>> => {
	const account = get(activeAccount);

	console.log('from clientfetch');
	console.log(account);

	const tokenResponse = await msalInstance.acquireTokenSilent({
		scopes: scopes,
		account: account?.account!
	});

	const bodyPayload: Record<string, any> = {
		action,
		data,
		activeAccount: {
			account: account?.account,
			token: tokenResponse.accessToken
		}
	};

	console.log('clientfetch body');
	console.log(bodyPayload);

	const response = await fetch(path, {
		method: 'POST',
		body: JSON.stringify(bodyPayload),
		headers: { 'Content-Type': 'application/json' }
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
	const json = await response.json();

	console.log('have json');
	//console.log(json);

	return {
		isSuccess: true,
		data: json
	} as Result<TResult>;
};
