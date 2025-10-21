import { redirect } from '@sveltejs/kit';
import { rpgSystemEndpoint } from './endpoint';

export const load = async ({ locals }: { locals: App.Locals }) => {
	console.log('RPG System layout server load - locals');
	console.log(locals);
	if (!locals.token) {
		console.log('No token found in rpgsystem route in locals, redirecting to login');
		throw redirect(302, '/login');
	}

	const response = await fetch(rpgSystemEndpoint, {
		headers: {
			Authorization: `Bearer ${locals.token}`
		}
	});

	const data = await response.json();

	return {
		rpgSystems: data
	};
};
