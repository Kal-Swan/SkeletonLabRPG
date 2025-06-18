import type { Character } from '../../../common/models/character/character-simple.js';
import { CharacterEndpoint } from '@environment/character/endpoint.js';
import { apiUrl } from '@environment/urls.js';
import type { characterSchemaType } from '@models/character/character-schema.js';

export async function load({ fetch, locals }) {
	console.log('locals in character', locals.token);
	try {
		// const configurationResponse = await fetch(`${apiUrl}/configuration`);
		// const configuration = (await configurationResponse.json()) as AppConfiguration;
		const res = await fetch(`${CharacterEndpoint}/getAll`, {
			headers: {
				Authorization: `Bearer ${locals.token}`
			}
		});
		const item = await res.json();

		return {
			characters: item as characterSchemaType[],
			configuration: null
		};
	} catch (error) {
		console.error(error);
		return {
			characters: [] as characterSchemaType[]
		};
	}

	return {
		characters: [] as Character[]
	};
}
