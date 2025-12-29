import z from 'zod';
import { buildRequest } from '../../../routes/(protected)/llm/build-details-schema';

export const buildSchema = z.object({
	id: z.string(),
	name: z.string(),
	template: z.string(),
	reason: z.string(),
	buildSystemId: z.string()
});

export type buildSchemaType = z.infer<typeof buildSchema>;
