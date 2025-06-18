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
} from '../../../common/helpers/server-fetch.js'; // Adjust path as needed

function validationRequestData<T extends ZodSchema>(
	data: unknown[],
	schema: T,
	actionName: string
): { success: boolean; data: z.infer<T>; error: Response | null } {
	const validationResult = schema.safeParse(data);
	if (!validationResult.success) {
		console.error(`Schema validation failed for ${actionName}: `, validationResult.error.flatten());
		//const formError = validationResult.error.flatten().formErrors.join(', ');
		// console.error(
		// 	Object.entries(validationResult.error.flatten().fieldErrors).flatMap((error) => ({
		// 		field: error[0],
		// 		message: error[1]?.join(', ')
		// 	}))
		// );
		const fieldErrors = Object.entries(validationResult.error.flatten().fieldErrors).flatMap(
			(error) => ({
				field: error[0],
				message: error[1]?.join(', ')
			})
		);
		// const fieldError = Object.entries(validationResult.error.flatten().fieldErrors)
		// 	.flatMap((error) => error[1]?.join(', '))
		// 	.join(', ');

		const errorResponse = json(
			{
				status: 400,
				message: `Validation failed for field ${fieldErrors[0].field} with error ${fieldErrors.map((e) => e.message).join(', ')}`
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

export async function POST({ request, locals }) {
	const { action, data } = await request.json();

	switch (action) {
		case Actions.createCharacter:
			console.log('create character post');
			console.log(data);

			var validationNewCharacterResult = validationRequestData(
				data,
				createCharacterSchema,
				Actions.createCharacter
			);

			if (!validationNewCharacterResult.success) {
				return validationNewCharacterResult.error!;
			}

			const createCharacterResponse = await serverPost(
				`${CharacterEndpoint}`,
				{ name: validationNewCharacterResult.data.name },
				locals.token
			);
			return createCharacterResponse;
		case Actions.getCharacterClasses:
			const classesResponse = await serverGet(
				`${CharacterClassEndpoint}/${data.id}/getall`,
				locals.token
			);
			if (classesResponse.status !== 200) {
				return json({ message: 'Failed to fetch classes' }, { status: classesResponse.status });
			}
			const classes = await classesResponse.json();
			return json(classes);
		case Actions.createCharacterClass:
			var validationCharacterClassResult = validationRequestData(
				data,
				createCharacterClassSchema,
				Actions.createCharacter
			);

			if (!validationCharacterClassResult.success) {
				return validationCharacterClassResult.error!;
			}

			const classResponse = await serverPost(
				`${CharacterClassEndpoint}/${validationCharacterClassResult.data.id}/createCharacterClass`,
				{
					name: validationCharacterClassResult.data.name,
					classType: validationCharacterClassResult.data.classType
				},
				locals.token
			);

			if (classResponse.status !== 201 && classResponse.status !== 200) {
				return json(
					{ message: 'Failed to create character class' },
					{ status: classResponse.status }
				);
			}
			return json({}, { status: 200 });
		case Actions.updateCharacterName:
			var validationUpdateCharacterResult = validationRequestData(
				data,
				characterSchema,
				Actions.updateCharacterName
			);

			if (!validationUpdateCharacterResult.success) {
				return validationUpdateCharacterResult.error!;
			}

			console.log('update character name post');
			console.log(validationUpdateCharacterResult.data);
			console.log(data.id);

			const updatedResponse = await serverPut(
				`${CharacterEndpoint}/${data.id}/updatename`,
				{ name: validationUpdateCharacterResult.data.name },
				locals.token
			);

			if (updatedResponse.status !== 201 && updatedResponse.status !== 200) {
				let errorText = 'Failed to update character name';
				try {
					const errorJson = await updatedResponse.json();
					errorText = errorJson.message || errorText;
				} catch (e) {
					errorText = (await updatedResponse.text()) || errorText;
				}
				console.error('Failed to update character name', updatedResponse.status, errorText);
				return json({ message: errorText }, { status: updatedResponse.status });
			}

			const result = await updatedResponse.json();

			console.log('Character name updated successfully', result);

			return json(result, { status: 200 });
		case Actions.deleteCharacter:
			console.log('delete character post');
			console.log(data);
			const response = await serverDelete(`${CharacterEndpoint}/${data.id}`, locals.token);

			if (response.status !== 200) {
				throw new Error('Failed to delete character');
			}

			return json(true);
		case Actions.createCharacterAttribute:
			var validateCharacterAttributeResult = validationRequestData(
				data,
				z.array(CharacterAttributeDamageSchema),
				Actions.createCharacterAttribute
			);

			if (!validateCharacterAttributeResult.success) {
				return validateCharacterAttributeResult.error!;
			}

			const lastIndex = validateCharacterAttributeResult.data.length - 1;

			const request = {
				favourite: validateCharacterAttributeResult.data[lastIndex],
				others: validateCharacterAttributeResult.data.slice(0, lastIndex)
			};

			const characterAttributeResponse = await serverPost(
				`${CharacterAttributeEndpoint}/${request.favourite.characterClassId}`,
				request,
				locals.token
			);

			if (characterAttributeResponse.status !== 201 && characterAttributeResponse.status !== 200) {
				return json(
					{ message: 'Failed to save character attribute' },
					{ status: characterAttributeResponse.status }
				);
			}
			return json({}, { status: 200 });
	}
}
