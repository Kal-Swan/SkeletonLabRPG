import type { AccountInfo, PublicClientApplication } from '@azure/msal-browser';
import { writable } from 'svelte/store';

export const defaultScopes = ['openid', 'profile'];
export const loggedIn = writable<boolean>(false);
export const accessTokenStore = writable<string | null>(null);
export const msalInstanceStore = writable<PublicClientApplication>();
export const activeAccount = writable<{
	account: AccountInfo | null;
	token: string | null;
} | null>(null);
export const msalReady = writable(false);
