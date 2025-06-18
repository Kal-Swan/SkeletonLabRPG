import { ClassType } from '@models/class-type';
import { z } from 'zod';

export const CharacterClassSchema = z.object({
	characterId: z.string(),
	id: z.string(),
	classType: z.nativeEnum(ClassType),
	name: z.string()
});

export const createCharacterClassSchema = z.object({
	id: z.string(),
	name: z.string(),
	classType: z.nativeEnum(ClassType)
});

export type createCharacterClassSchemaType = z.infer<typeof createCharacterClassSchema>;

export type CharacterClassSchemaType = z.infer<typeof CharacterClassSchema>;
