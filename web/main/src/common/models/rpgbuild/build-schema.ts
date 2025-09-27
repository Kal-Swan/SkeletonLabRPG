import z from 'zod';

export const buildSchema = z.object({
	id: z.string(),
	name: z.string(),
	template: z.string(),
	reason: z.string(),
	rpgSystemId: z.string()
});

export type buildSchemaType = z.infer<typeof buildSchema>;
