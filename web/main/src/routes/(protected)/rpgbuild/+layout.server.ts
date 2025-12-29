import { getAllRpgBuildsEndpoint } from './endpoint';
import { redirect } from '@sveltejs/kit';
import { fetchHandler } from '@helpers/error-helper.js';

export const load = async ({ locals }: {locals: { token: string }}) => {
	if (!locals.token) {
		throw redirect(302, '/login');
	}
	console.log('loading builds');
	// const response = await fetch(getAllRpgBuildsEndpoint, {
	// 	headers: {
	// 		Authorization: `Bearer ${locals.token}`
	// 	}
	// });
	// const builds = await response.json();

	const builds = await fetchHandler(getAllRpgBuildsEndpoint, locals.token);

	console.log('saved builds');
	console.log(builds);

	return {
		builds: builds
	};
};
