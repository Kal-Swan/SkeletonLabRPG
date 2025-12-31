import { redirect } from '@sveltejs/kit';
import { rpgSystemEndpoint } from './endpoint';
import { fetchHandler } from '@helpers/error-helper.js';

export const load = async ({ locals }: { locals: App.Locals }) => {
	console.log('RPG System layout server load - locals');
	console.log(locals);
	if (!locals.token) {
		console.log('No token found in rpgsystem route in locals, redirecting to login');
		throw redirect(302, '/login');
	}

	console.log('Fetching RPG systems from endpoint with token');
	console.log(locals.token);

	// try {
	// 	const response = await fetch(rpgSystemEndpoint, {
	// 		headers: {
	// 			Authorization: `Bearer ${locals.token}`
	// 		}
	// 	});
	
	// 	const data = await response.json();
	
	// 	return {
	// 		rpgSystems: data
	// 	};
	// } catch (error) {
	// 	console.error('Error fetching RPG systems:', error);
	// 	return {
	// 		rpgSystems: []
	// 	};
	// }

	const rpgSystems = await fetchHandler(rpgSystemEndpoint, locals.token);
	return {
		rpgSystems: rpgSystems
	};
};
