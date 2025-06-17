interface ServerFetchOptions extends Omit<RequestInit, 'headers'> {
	headers?: Record<string, string>;
	token?: string;
}

async function baseServerFetch(
	path: string,
	options: RequestInit,
	token?: string | null
): Promise<Response> {
	const headers = new Headers(options.headers);
	if (token) {
		headers.set('Authorization', `Bearer ${token}`);
	}
	return fetch(path, { ...options, headers });
}

export async function serverGet(path: string, token?: string | null): Promise<Response> {
	return baseServerFetch(
		path,
		{
			method: 'GET'
		},
		token
	);
}

export async function serverPost(
	path: string,
	data: Record<string, any>,
	token: string | null
): Promise<Response> {
	return baseServerFetch(
		path,
		{
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(data)
		},
		token
	);
}

export async function serverPut(
	path: string,
	data: Record<string, any>,
	token?: string | null
): Promise<Response> {
	return baseServerFetch(
		path,
		{
			method: 'PUT',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(data)
		},
		token
	);
}

export async function serverDelete(path: string, token?: string | null): Promise<Response> {
	return baseServerFetch(
		path,
		{
			method: 'DELETE'
		},
		token
	);
}
