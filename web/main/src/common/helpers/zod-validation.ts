import { json } from '@sveltejs/kit';
import type { z, ZodSchema } from 'zod';

export function validateRequest<T extends ZodSchema>(
	data: unknown,
	schema: T,
	actionName: string
): { success: boolean; data: z.infer<T>; error: Response | null } {
	const validationResult = schema.safeParse(data);
	console.log('validationResult');
	console.log(validationResult.error);
	if (!validationResult.success) {
		console.error(`Schema validation failed for ${actionName}: `, validationResult.error.flatten());

		const fieldErrors = Object.entries(validationResult.error.flatten().fieldErrors).flatMap(
			(error) => ({
				field: error[0],
				message: error[1]?.join(', ')
			})
		);

		const errorResponse = json(
			{
				status: 400,
				message: `Validation failed for field ${fieldErrors[0].field} with error "${fieldErrors.map((e) => e.message).join(', ')}"`
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
