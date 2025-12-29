import { json } from '@sveltejs/kit';
import { serverPost as serverDefaultPostFetch } from '@helpers/server-fetch.js';
import { createBuildQuestionSchema } from '@models/rpgbuild/llm-schema.js';
import { buildSchema } from '@models/rpgbuild/build-schema.js';
import { validateRequest } from '@helpers/zod-validation.js';
import type { ActiveAccount } from '@models/auth/account.js';
import { createBuildEndpoint, updateBuildRequestEndpoint } from './endpoint';
import { Actions } from './actions.js';
import { BuildAnswerStatus, buildRequestAnswer } from './build-details-schema';

export async function POST({ request }: any) {
	console.log('server side llm');
	const { action, data, activeAccount } = await request.json();

	console.log(action);
	console.log(activeAccount);
	switch (action) {
		case Actions.createBuildRequest:
			console.log('before validation');
			var validationResult = validateRequest(
				data,
				createBuildQuestionSchema,
				Actions.createBuildRequest
			);

			if (!validationResult.success) {
				return validationResult.error!;
			}

			console.log('after validation');

			const response = await serverDefaultPostFetch(
				`${createBuildEndpoint}`,
				{
					question: validationResult.data.question,
					buildSystemId: validationResult.data.buildSystemId
				},
				activeAccount
			);
			console.log('after server post response');
			return response;
		case Actions.updateBuildRequest:
			var validation = validateRequest(data, buildRequestAnswer, Actions.updateBuildRequest);

			if (!validation.success) {
				return validation.error!;
			}

			const updateResponse = await serverDefaultPostFetch(
				`${updateBuildRequestEndpoint}/${validation.data.buildRequestId}/${validation.data.id}?status=${validation.data.status!}`,
				validation.data,
				activeAccount,
				'PUT'
			);
			return updateResponse;
	}
}
