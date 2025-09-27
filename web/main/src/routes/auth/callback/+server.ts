import { json } from '@sveltejs/kit';

export async function POST({ request, cookies, locals }) {
	const { token } = await request.json();
	locals.token = token;

	return json({ success: true });
}
