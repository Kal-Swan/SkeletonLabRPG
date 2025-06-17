export interface ErrorResponse {
	message: string;
	statusCode: number;
	type: number;
	fieldErrors?: Record<string, string[]>;
}
