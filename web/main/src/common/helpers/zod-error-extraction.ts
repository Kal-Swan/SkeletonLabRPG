import type z from 'zod';

export const zodFieldErrorExtraction = (
	zodError: z.ZodError<any>
): {
	field: string;
	message: string;
}[] => {
	return zodError.errors.map((err) => ({ field: err.path[0].toString(), message: err.message }));
};
