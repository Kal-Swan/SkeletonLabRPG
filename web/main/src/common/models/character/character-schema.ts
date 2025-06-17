import { z } from 'zod';

export const characterSchema = z.object({
	id: z.string(),
	name: z.string()
});

export const updateCharacterSchema = z.object({
	name: z.string()
});

export const createCharacterSchema = z.object({
	name: z.string().min(1, 'Name is required')
});

export type characterSchemaType = z.infer<typeof characterSchema>;
export type createCharacterSchemaType = z.infer<typeof createCharacterSchema>;
export type updateCharacterSchemaType = z.infer<typeof updateCharacterSchema>;
