import { redirect } from '@sveltejs/kit';

export const load = async ({ locals }) => {
	// e.g. your auth state lives in locals.user
	console.log('locals', locals);
	if (!locals.token) {
		throw redirect(302, '/login');
	}
	return {};
};
