import { Actions, type ActionType } from '@models/actions';
import type { ErrorResponse } from '@models/error-response';
import { error } from '@sveltejs/kit';

export const errorMessageSplitByComma = (errors: ErrorResponse[] | undefined) => {
	if (errors?.length === 0) {
		return '';
	}
	const errorMessages = errors!.map((error) => error.message);
	return errorMessages.join(', ');
};

export const logError = (errors: ErrorResponse[] | undefined) => {
	const error = errorMessageSplitByComma(errors);
	if (error) {
		console.error('Error:', error);
	}
};

export const isSingleFieldAction = (action: ActionType) => {
	const singleFieldActions = [
		Actions.createCharacter,
		Actions.createCharacterClass,
		Actions.deleteAllCharacters,
		Actions.deleteCharacter,
		Actions.updateCharacter
	];
	return singleFieldActions.includes(action);
};

export const fetchHandler = async (url: string, token: string) => {
	try {
		const res = await fetch(url, {
			headers: { Authorization: `Bearer ${token}` }
		});

		return await res.json();
	} catch (err) {
		if ((err as any)?.status) throw err;
		throw error(503, `Unable to reach API at ${url}`);
	}
}
