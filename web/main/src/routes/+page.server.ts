import { redirect } from '@sveltejs/kit';

export const load = async () => {
	// Always redirect from the root path ("/") to "/login"
	throw redirect(302, '/login');
};
