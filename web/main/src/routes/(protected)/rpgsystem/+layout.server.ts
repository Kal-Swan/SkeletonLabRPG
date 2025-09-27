import { redirect } from '@sveltejs/kit';
import { rpgSystemEndpoint } from './endpoint';

export const load = async ({ locals }) => {
	if (!locals.token) {
		console.log('error');
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
