export async function POST({ locals }) {
	locals.token = null;
	return new Response(null, { status: 200 });
}
