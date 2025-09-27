import z from 'zod';

export const rpgSystemSchema = z.object({
	id: z.string(),
	name: z.string().min(1, 'Name is required')
});

export const createRpgSystemSchema = z.object({
	name: z.string().min(1, 'Name is required')
});

export type rpgSystemSchemaType = z.infer<typeof rpgSystemSchema>;
export type createRpgSystemSchemaType = z.infer<typeof createRpgSystemSchema>;
