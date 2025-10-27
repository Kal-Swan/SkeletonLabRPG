import { getAllRpgBuildsEndpoint } from '@environment/rpg-build/endpoint';
import { redirect } from '@sveltejs/kit';

export const load = async ({ locals }: {locals: { token: string }}) => {
	if (!locals.token) {
		throw redirect(302, '/login');
	}
	console.log('loading builds');
	const response = await fetch(getAllRpgBuildsEndpoint, {
		headers: {
			Authorization: `Bearer ${locals.token}`
		}
	});
	const builds = await response.json();

	console.log('saved builds');
	console.log(builds);

	return {
		builds: builds
	};
};
