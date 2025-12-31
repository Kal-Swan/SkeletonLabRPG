import z from 'zod';
import { id } from 'zod/v4/locales';

export const llmBuildSchema = z.object({
	id: z.string().optional(),
	name: z.string(),
	reason: z.string(),
	template: z.string()
});

export const rpgBuildQuestionSchema = z.object({
	buildSystemId: z.string().min(1),
	question: z.string().min(1)
});

export const createBuildQuestionSchema = z.object({
	buildSystemId: z.string(),
	question: z.string().min(1)
});

export const answers = z.array(llmBuildSchema);

export const llmBuildsSchema = z.array(llmBuildSchema);
export const llmBuildResponseSchema = z.object({
	id: z.string(),
	buildSystemId: z.string(),
	answers: answers
});

export type llmBuildResponseType = z.infer<typeof llmBuildResponseSchema>;
export type llmBuildSchemaType = z.infer<typeof llmBuildSchema>;
export type llmBuildsSchemaType = z.infer<typeof llmBuildsSchema>;
