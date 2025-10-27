import { redirect } from '@sveltejs/kit';

export async function load({ locals }: { locals: { isAuthenticated: boolean } }) {
  if (!locals.isAuthenticated) throw redirect(302, '/login');
  return {};
}