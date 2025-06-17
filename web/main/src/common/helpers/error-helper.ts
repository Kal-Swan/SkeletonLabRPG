import { Actions, type ActionType } from '@models/actions';
import type { ErrorResponse } from '@models/error-response';

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
