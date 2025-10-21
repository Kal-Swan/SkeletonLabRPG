export async function POST({ locals }: { locals: any }) {
	locals.token = null;
	return new Response(null, { status: 200 });
}
