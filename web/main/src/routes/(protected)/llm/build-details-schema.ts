import z from 'zod';
import { id } from 'zod/v4/locales';

export enum BuildRequestStatus {
	None = 0,
	Processing = 10,
	Completed = 20
}

export enum BuildAnswerStatus {
	None = 0,
	Saved = 10,
	Deleted = 20
}

export const answer = z.object({
	id: z.string(),
	name: z.string(),
	reason: z.string(),
	template: z.string(),
	status: z.nativeEnum(BuildAnswerStatus).default(BuildAnswerStatus.None).optional()
});

export const buildRequestAnswer = answer.extend({
	buildRequestId: z.string()
});

export const buildRequest = z.object({
	id: z.string(),
	question: z.string(),
	buildSystemId: z.string(),
	buildSystemName: z.string(),
	status: z.nativeEnum(BuildRequestStatus),
	answers: z.array(answer),
	latestProcessedDate: z.date()
});

export const buildSystem = z.object({
	id: z.string(),
	name: z.string()
});

export const buildDetails = z.object({
	buildRequests: z.array(buildRequest),
	buildSystems: z.array(buildSystem)
});

export type buildDetailsSchema = z.infer<typeof buildDetails>;
export type buildRequestType = z.infer<typeof buildRequest>;
export type buildSystemType = z.infer<typeof buildSystem>;
export type buildAnswerType = z.infer<typeof answer>;
export type buildRequestAnswerType = z.infer<typeof buildRequestAnswer>;
