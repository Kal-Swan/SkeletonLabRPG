import z from 'zod';

const FileMaxSize = 52_428_800; // 50MB

export const buildSystemSchema = z
	.object({
		id: z.string(),
		name: z.string().min(1, 'Name is required'),
		fileNames: z.array(z.string()).optional(),
		files: z
			.array(
				z
					.instanceof(File)
					.refine((file) => ['application/pdf', 'text/plain'].includes(file.type), {
						message: 'Invalid file type, only PDF and TXT are allowed'
					})
					.refine((file) => file.size <= FileMaxSize, { message: 'File size exceeds 50MB' })
			)
			.optional()
	})
	.refine(
		(schema) => {
			if (!schema.fileNames?.length && !schema.files?.length) {
				return false;
			}
			return true;
		},
		{
			message: 'At least one file is required',
			path: ['files']
		}
	);

export const createBuildSystemSchema = z.object({
	name: z.string().min(1, 'Name is required'),
	files: z
		.array(
			z
				.instanceof(File)
				.refine((file) => ['application/pdf', 'text/plain'].includes(file.type), {
					message: 'Invalid file type, only PDF and TXT are allowed'
				})
				.refine((file) => file.size <= FileMaxSize, { message: 'File size exceeds 50MB' })
		)
		.min(1, 'At least one file is required')
});

export const openBuildSystemFileSchema = z.object({
	id: z.string(),
	fileName: z.string()
});

export type buildSystemSchemaType = z.infer<typeof buildSystemSchema>;
export type createBuildSystemSchemaType = z.infer<typeof createBuildSystemSchema>;
