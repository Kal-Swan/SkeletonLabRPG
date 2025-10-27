import { json } from '@sveltejs/kit';
import { Actions } from '@models/actions.js';
import { z, ZodSchema } from 'zod';
import { serverDelete, serverPost, serverPut } from '@helpers/server-fetch.js';
import { buildSchema } from '@models/rpgbuild/build-schema.js';
import { baseBuildsEndpoint, deleteRpgBuildEndpoint, updateRpgBuildEndpoint } from '@environment/rpg-build/endpoint.js';
import type { ActiveAccount } from '@models/auth/account.js';

function validationRequestData<T extends ZodSchema>(
	data: unknown,
	schema: T,
	actionName: string
): { success: boolean; data: z.infer<T>; error: Response | null } {
	const validationResult = schema.safeParse(data);
	if (!validationResult.success) {
		const fieldErrors = Object.entries(validationResult.error.flatten().fieldErrors).flatMap(
			(error) => ({
				field: error[0],
				message: ` ${error[0]} - ${error[1]} `
			})
		);

		const errorResponse = json(
			{
				status: 400,
				message: `Validation failed: ${fieldErrors.map((e) => e.message)}`
			},
			{ status: 400 }
		);
		return {
			success: false,
			data: null,
			error: errorResponse
		};
	}

	return { success: true, data: validationResult.data, error: null };
}

export async function POST({ request } : any) {
	console.log('server side rpg build');
	const { action, data, activeAccount } = await request.json();

	console.log(action);
	console.log(activeAccount);
	console.log(data);

	switch (action) {
		case Actions.updateRpgBuild:
			return updateRpgBuild(data, activeAccount);
		case Actions.deleteRpgBuild:
			return deleteRpgBuild(data, activeAccount);
	}
}

const updateRpgBuild = async (data: any, activeAccount: ActiveAccount) => {
	var validation = validationRequestData(data, buildSchema, Actions.updateRpgBuild);

	if (!validation.success) {
		return validation.error!;
	}

	const response = await serverPut(
		`${updateRpgBuildEndpoint}/${validation.data.id}`,
		validation.data,
		activeAccount
	);
	return response;
};

const deleteRpgBuild = async (data: any, activeAccount: ActiveAccount) => {
	const response = await serverDelete(`${baseBuildsEndpoint}/${data.id}`, activeAccount);
	return response;
};
