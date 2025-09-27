import type { ActiveAccount } from '@models/auth/account';

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

export async function serverGet(path: string, activeAccount: ActiveAccount): Promise<Response> {
	if (activeAccount?.account == null) {
		throw new Error('No active account - user not signed in');
	}

	return baseServerFetch(
		path,
		{
			method: 'GET'
		},
		activeAccount.token
	);
}

export async function serverPost(
	path: string,
	data: Record<string, any>,
	activeAccount: ActiveAccount
): Promise<Response> {
	console.log('server post active account');
	console.log(activeAccount?.account);
	if (activeAccount?.account == null) {
		throw new Error('No active account - user not signed in');
	}

	return baseServerFetch(
		path,
		{
			method: 'POST',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(data)
		},
		activeAccount.token
	);
}

export async function serverPut(
	path: string,
	data: Record<string, any>,
	activeAccount: ActiveAccount
): Promise<Response> {
	if (activeAccount?.account == null) {
		throw new Error('No active account - user not signed in');
	}
	return baseServerFetch(
		path,
		{
			method: 'PUT',
			headers: { 'Content-Type': 'application/json' },
			body: JSON.stringify(data)
		},
		activeAccount.token
	);
}

export async function serverDelete(path: string, activeAccount: ActiveAccount): Promise<Response> {
	if (activeAccount?.account == null) {
		throw new Error('No active account - user not signed in');
	}
	return baseServerFetch(
		path,
		{
			method: 'DELETE'
		},
		activeAccount.token
	);
}
