import { redirect } from '@sveltejs/kit';

export const load = async (data) => {
	console.log('login page load');
	console.log(data.locals);
	if (data.locals.isAuthenticated) {
		throw redirect(302, '/rpgsystem');
	}
};
