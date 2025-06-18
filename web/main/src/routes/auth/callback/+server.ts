import { json } from '@sveltejs/kit';

export async function POST({ request, cookies }) {
	const { token } = await request.json();

	console.log('in post callback');
	console.log('token', token);

	cookies.set('auth_token', token, {
		httpOnly: true,
		secure: true,
		sameSite: 'lax',
		path: '/',
		maxAge: 3600
	});

	return json({ success: true });
}

// import type { RequestHandler } from '@sveltejs/kit';

// export const POST: RequestHandler = async ({ request, cookies }) => {
// 	const { token } = await request.json();

// 	console.log('in post callback');
// 	console.log('token', token);

// 	cookies.set('auth_token', token, {
// 		httpOnly: true,
// 		secure: true,
// 		sameSite: 'lax',
// 		path: '/',
// 		maxAge: 3600
// 	});

// 	return new Response(JSON.stringify({ success: true }));
// };
