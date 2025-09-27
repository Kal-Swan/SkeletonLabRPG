import { json } from '@sveltejs/kit';
import {
	CharacterAttributeEndpoint,
	CharacterClassEndpoint,
	CharacterEndpoint
} from '@environment/character/endpoint.js';
import { Actions } from '@models/actions.js';
import { z, ZodSchema, type typeToFlattenedError } from 'zod';
import { createCharacterClassSchema } from '@models/character/classes/character-class-schema.js';
import { characterSchema, createCharacterSchema } from '@models/character/character-schema.js';
import {
	CharacterAttributeDamageSchema,
	CharacterAttributeSchema
} from '@models/character/character-attribute-schema.js';
import type { ErrorResponse } from '@models/error-response.js';
import {
	serverGet,
	serverPost,
	serverPut,
	serverDelete
} from '../../../common/helpers/server-fetch.js';
import { createBuildQuestionSchema } from '@models/rpgbuild/llm-schema.js';
import { CreateRpgBuildsEndpoint } from '@environment/llm/endpoints.js';
import { createRpgBuildEndpoint } from '@environment/build/endpoint.js';
import { buildSchema } from '@models/rpgbuild/build-schema.js';
import { validateRequest } from '@helpers/zod-validation.js';
import type { ActiveAccount } from '@models/auth/account.js';

export async function POST({ request, locals }) {
	const { action, data, activeAccount } = await request.json();

	switch (action) {
		case Actions.createRpgBuilds:
			var validationResult = validateRequest(
				data,
				createBuildQuestionSchema,
				Actions.createRpgBuilds
			);

			if (!validationResult.success) {
				return validationResult.error!;
			}

			const response = await serverPost(
				`${CreateRpgBuildsEndpoint}`,
				{ question: validationResult.data.question, rpgSystem: validationResult.data.rpgSystem },
				activeAccount
			);

			return response;
		case Actions.saveRpgBuild:
			return saveRpgBuild(data, activeAccount);
	}
}

const saveRpgBuild = async (data: any, activeAccount: ActiveAccount) => {
	var validation = validateRequest(data, buildSchema, Actions.saveRpgBuild);

	if (!validation.success) {
		return validation.error!;
	}

	const response = await serverPost(`${createRpgBuildEndpoint}`, validation.data, activeAccount);
	return response;
};
