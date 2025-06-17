export const Actions = {
	getCharacterClasses: 'getCharacterClasses',
	createCharacterClass: 'createCharacterClass',
	updateCharacter: 'updateCharacter',
	deleteCharacter: 'deleteCharacter',
	createCharacter: 'createCharacter',
	deleteAllCharacters: 'deleteAllCharacters',
	createCharacterAttribute: 'createCharacterAttribute',
	updateCharacterName: 'updateCharacterName'
};

export type ActionType = (typeof Actions)[keyof typeof Actions];
