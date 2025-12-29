import { getAllRpgBuildsEndpoint } from '../rpgbuild/endpoint.js';
import { redirect } from '@sveltejs/kit';
import { rpgSystemEndpoint } from '../rpgsystem/endpoint.js';
import { buildDetailsEndpoint } from './endpoint.js';
import { fetchHandler } from '@helpers/error-helper.js';
import type { buildDetailsSchema } from './build-details-schema.js';

export const load = async ({ locals }) => {
	if (!locals.token) {
		throw redirect(302, '/login');
	}

	// const response = await fetch(rpgSystemEndpoint, {
	// 	headers: {
	// 		Authorization: `Bearer ${locals.token}`
	// 	}
	// });

	// const data = await response.json();

	// const buildSystems = await fetchHandler(rpgSystemEndpoint, locals.token);
	// const buildRequests = await fetchHandler(buildDetailsEndpoint, locals.token);
	const buildDetails: buildDetailsSchema = await fetchHandler(buildDetailsEndpoint, locals.token);
	console.log('build details loaded');
	console.log(buildDetails.buildRequests.flatMap((br) => br.answers));

	// const processingRequests = await fetch(processingRequestsEndpoint, {
	// 	headers: {
	// 		Authorization: `Bearer ${locals.token}`
	// 	}
	// });

	return {
		buildSystems: buildDetails.buildSystems,
		buildRequests: buildDetails.buildRequests
	};
};
