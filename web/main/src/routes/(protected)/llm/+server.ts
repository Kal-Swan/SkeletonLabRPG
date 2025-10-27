import { json } from '@sveltejs/kit';
import { Actions } from '@models/actions.js';
import { serverPost } from '@helpers/server-fetch.js';
import { createBuildQuestionSchema } from '@models/rpgbuild/llm-schema.js';
import { CreateRpgBuildsEndpoint } from '@environment/llm/endpoints.js';
import { createRpgBuildEndpoint } from '@environment/rpg-build/endpoint.js';
import { buildSchema } from '@models/rpgbuild/build-schema.js';
import { validateRequest } from '@helpers/zod-validation.js';
import type { ActiveAccount } from '@models/auth/account.js';

export async function POST({ request, locals }) {
	console.log('server side llm');
	const { action, data, activeAccount } = await request.json();

	console.log(action);
	console.log(activeAccount);
	switch (action) {
		case Actions.createRpgBuilds:
			console.log('before validation');
			var validationResult = validateRequest(
				data,
				createBuildQuestionSchema,
				Actions.createRpgBuilds
			);


			if (!validationResult.success) {
				return validationResult.error!;
			}

			console.log('after validation');

			const response = await serverPost(
				`${CreateRpgBuildsEndpoint}`,
				{ question: validationResult.data.question, rpgSystem: validationResult.data.rpgSystem },
				activeAccount
			);
			console.log('after server post response');
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
