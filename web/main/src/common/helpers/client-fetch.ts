import { type ActionType } from '@models/actions.js';
import type { ErrorResponse } from '@models/error-response';
import { errorMessage } from '@lib/stores/error-message';
import { isSingleFieldAction } from './error-helper';

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
	const bodyPayload: Record<string, any> = { action, data };

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
		errorMessage.set([error.message]);
		return { isSuccess: false };
	}

	if (!response.ok && !isSingleFieldAction(action)) {
		const error = (await response.json()) as ErrorResponse;

		if (error.fieldErrors) {
			console.error(
				`Failed to action ${action} with status: ${response.status} and error: ${error.fieldErrors}`
			);
		}

		errorMessage.set([error.message ?? 'Failed to action ' + action]);
		return { isSuccess: false, fieldErrors: error.fieldErrors };
	}

	const json = await response.json();

	console.log('clientfetch json');
	console.log(json);

	return {
		isSuccess: true,
		data: json
	} as Result<TResult>;
};
