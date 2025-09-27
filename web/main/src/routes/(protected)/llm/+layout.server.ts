import { getAllRpgBuildsEndpoint } from '@environment/build/endpoint';
import { redirect } from '@sveltejs/kit';
import { rpgSystemEndpoint } from '../rpgsystem/endpoint.js';

export const load = async ({ locals }) => {
	if (!locals.token) {
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
