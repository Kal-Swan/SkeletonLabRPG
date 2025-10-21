import { json, type RequestEvent } from '@sveltejs/kit';

export async function POST({ request, locals }: RequestEvent) {
	const { token } = await request.json();
	console.log('Setting token in server auth callback');
	console.log(token);
	locals.token = token;

	return json({ success: true });
}
