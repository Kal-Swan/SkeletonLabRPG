import { json } from '@sveltejs/kit';
import { Actions } from '@models/actions.js';
import { serverPost, serverPut, serverDelete, baseServerFetch } from '@helpers/server-fetch.js';
import { validateRequest } from '@helpers/zod-validation.js';
import { rpgSystemEndpoint } from './endpoint.js';
import {
	createBuildSystemSchema,
	buildSystemSchema,
	type buildSystemSchemaType
} from '@models/rpgbuild/rpg-system-schema.js';
import type { ActiveAccount } from '@models/auth/account.js';

export async function POST({ request }) {
	const contentType = request.headers.get('content-type') ?? '';

	let action: string;
	let data: Record<string, unknown>;
	let activeAccount: ActiveAccount;

	if (contentType.includes('multipart/form-data')) {
		const formData = await request.formData();

		action = formData.get('action') as string;

		activeAccount = JSON.parse(formData.get('activeAccount') as string);

		data = {};

		for (const [key, value] of formData.entries()) {
			if (!key.startsWith('data.')) continue;

			const dataKey = key.replace('data.', '');

			// File objects stay as File
			data[dataKey] = value;
		}
	} else {
		const json = await request.json();

		action = json.action;
		data = json.data;
		activeAccount = json.activeAccount;
	}

	switch (action) {
		case Actions.createRpgSystem:
			console.log('Creating RPG System with data:', data);

			const normalisedData = normalizeMultipartData(data);

			const newRpgSystem = validateRequest(
				normalisedData,
				createBuildSystemSchema,
				Actions.createRpgSystem
			);

			if (!newRpgSystem.success) {
				return newRpgSystem.error!;
			}

			const formData = convertNormalisedDataToFormData(
				newRpgSystem.data.name,
				newRpgSystem.data.files as File[]
			);

			// const files = normalisedData.files as File[];

			// const formData = new FormData();
			// formData.append('name', newRpgSystem.data.name);
			// files.forEach((file) => {
			// 	formData.append('files', file);
			// });

			console.log('formdata');
			console.log(formData);

			const headers = new Headers();

			if (activeAccount.token) {
				headers.set('Authorization', `Bearer ${activeAccount.token}`);
			}

			const createdRpgSystem = await fetch(`${rpgSystemEndpoint}`, {
				body: formData,
				headers,
				method: 'POST'
			});

			// const createdRpgSystem = await serverPost(
			// 	`${rpgSystemEndpoint}`,
			// 	formData,
			// 	activeAccount,
			// 	'POST',
			// 	''
			// );
			return createdRpgSystem;
		case Actions.updateRpgSystem: {
			console.log('updating rpg system');
			console.log(data);
			const normalisedData = normalizeMultipartData(data) as buildSystemSchemaType;
			var rpgSystemData = validateRequest(
				normalisedData,
				buildSystemSchema,
				Actions.updateRpgSystem
			);

			if (!rpgSystemData.success) {
				return rpgSystemData.error!;
			}

			const headers = new Headers();

			if (activeAccount.token) {
				headers.set('Authorization', `Bearer ${activeAccount.token}`);
			}

			const formData = convertNormalisedDataToFormData(
				normalisedData.name,
				normalisedData.files,
				normalisedData.fileNames
			);

			const updatedResponse = await fetch(`${rpgSystemEndpoint}/${data.id}`, {
				body: formData,
				headers,
				method: 'PUT'
			});

			// const updatedResponse = await serverPut(
			// 	`${rpgSystemEndpoint}/${data.id}`,
			// 	{
			// 		name: rpgSystemData.data.name,
			// 		fileNames: rpgSystemData.data.fileNames,
			// 		files: rpgSystemData.data.files
			// 	},
			// 	activeAccount
			// );

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
		}
		case Actions.deleteRpgSystem:
			const response = await serverDelete(`${rpgSystemEndpoint}/${data.id}`, activeAccount);

			if (response.status !== 200) {
				throw new Error('Failed to delete rpg system');
			}

			return json(true);
		case Actions.openBuildSystemFile:
			const fileResponse = await baseServerFetch(
				`${rpgSystemEndpoint}/${data.id}/${data.fileName}`,
				{
					method: 'GET'
				},
				activeAccount.token
			);

			if (fileResponse.status !== 200) {
				throw new Error('Failed to retrieve build system file');
			}

			return fileResponse;
	}
}

function convertNormalisedDataToFormData(
	name: string,
	files?: File[],
	fileNames?: string[]
): FormData {
	const formData = new FormData();
	formData.append('name', name);
	files?.forEach((file) => {
		formData.append('files', file);
	});
	fileNames?.forEach((fileName) => {
		formData.append('fileNames', fileName);
	});
	return formData;
}

function normalizeMultipartData(input: Record<string, unknown>): Record<string, unknown> {
	const result: Record<string, unknown> = {};

	for (const [key, value] of Object.entries(input)) {
		const arrayMatch = key.match(/^(.+)\[(\d+)\]$/);

		if (arrayMatch) {
			const [, arrayKey, index] = arrayMatch;
			const idx = Number(index);

			if (!Array.isArray(result[arrayKey])) {
				result[arrayKey] = [];
			}

			(result[arrayKey] as unknown[])[idx] = value;
		} else {
			result[key] = value;
		}
	}

	return result;
}
