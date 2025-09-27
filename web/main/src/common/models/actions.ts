export const Actions = {
	getCharacterClasses: 'getCharacterClasses',
	createCharacterClass: 'createCharacterClass',
	updateCharacter: 'updateCharacter',
	deleteCharacter: 'deleteCharacter',
	createCharacter: 'createCharacter',
	deleteAllCharacters: 'deleteAllCharacters',
	createCharacterAttribute: 'createCharacterAttribute',
	updateCharacterName: 'updateCharacterName',
	createRpgBuilds: 'createRpgBuilds',
	saveRpgBuild: 'saveRpgBuild',
	getAllRpgBuilds: 'getAllRpgBuilds',
	updateRpgBuild: 'updateRpgBuild',
	deleteRpgBuild: 'deleteRpgBuild',
	createRpgSystem: 'createRpgSystem',
	updateRpgSystem: 'updateRpgSystem',
	deleteRpgSystem: 'deleteRpgSystem'
};

export type ActionType = (typeof Actions)[keyof typeof Actions];
