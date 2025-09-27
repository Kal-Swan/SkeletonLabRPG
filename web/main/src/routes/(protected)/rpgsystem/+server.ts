import { json } from '@sveltejs/kit';
import { Actions } from '@models/actions.js';
import { serverPost, serverPut, serverDelete } from '../../../common/helpers/server-fetch.js';
import { validateRequest } from '@helpers/zod-validation.js';
import { rpgSystemEndpoint } from './endpoint.js';
import {
	createRpgSystemSchema,
	rpgSystemSchema
} from '../../../common/models/rpgbuild/rpg-system-schema.js';

export async function POST({ request }) {
	const { action, data, activeAccount } = await request.json();

	switch (action) {
		case Actions.createRpgSystem:
			var newRpgSystem = validateRequest(data, createRpgSystemSchema, Actions.createCharacter);

			if (!newRpgSystem.success) {
				return newRpgSystem.error!;
			}

			const createdRpgSystem = await serverPost(
				`${rpgSystemEndpoint}`,
				{ name: newRpgSystem.data.name },
				activeAccount
			);
			return createdRpgSystem;
		case Actions.updateRpgSystem:
			var rpgSystemData = validateRequest(data, rpgSystemSchema, Actions.updateRpgSystem);

			if (!rpgSystemData.success) {
				return rpgSystemData.error!;
			}

			const updatedResponse = await serverPut(
				`${rpgSystemEndpoint}/${data.id}`,
				{ name: rpgSystemData.data.name },
				activeAccount
			);

			if (updatedResponse.status !== 201 && updatedResponse.status !== 200) {
				let errorText = 'Failed to rpg system';
				try {
					const errorJson = await updatedResponse.json();
					errorText = errorJson.message || errorText;
				} catch (e) {
					errorText = (await updatedResponse.text()) || errorText;
				}
				console.error('Failed to update rpg system', updatedResponse.status, errorText);
				return json({ message: errorText }, { status: updatedResponse.status });
			}

			const result = await updatedResponse.json();

			return json(result, { status: 200 });
		case Actions.deleteRpgSystem:
			const response = await serverDelete(`${rpgSystemEndpoint}/${data.id}`, activeAccount);

			if (response.status !== 200) {
				throw new Error('Failed to delete rpg system');
			}

			return json(true);
	}
}
