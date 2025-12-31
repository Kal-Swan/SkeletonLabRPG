import type { AccountInfo, PublicClientApplication } from '@azure/msal-browser';
import { get, writable } from 'svelte/store';
import { configStore } from './config-store';

export const defaultScopes = ['openid', 'profile'];
export const loggedIn = writable<boolean>(false);
export const accessTokenStore = writable<string | null>(null);
export const msalInstanceStore = writable<PublicClientApplication>();
export const activeAccount = writable<{
	account: AccountInfo | null;
	token: string | null;
} | null>(null);
export const msalReady = writable(false);

export const getAccessToken = async () => {
	const account = get(activeAccount);
	const config = get(configStore);
	const msal = get(msalInstanceStore);
	const scopes = [...defaultScopes, config.b2c.apiAccessScope];
	const tokenResponse = await msal.acquireTokenSilent({
		scopes: scopes,
		account: account?.account!
	});
	return tokenResponse.accessToken;
}
