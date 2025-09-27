import {
	InteractionRequiredAuthError,
	PublicClientApplication,
	type AccountInfo
} from '@azure/msal-browser';
import * as v from '$env/dynamic/public';
import { writable } from 'svelte/store';

const msalConfig = {
	auth: {
		clientId: v.env.PUBLIC_AZURE_B2C_WEB_CLIENT_ID,
		authority: v.env.PUBLIC_AZURE_B2C_AUTHORITY,
		redirectUri: v.env.PUBLIC_AZURE_B2C_REDIRECT_URI,
		knownAuthorities: [v.env.PUBLIC_AZURE_B2C_TENANT],
		navigateToLoginRequestUrl: false
	},
	cache: {
		cacheLocation: 'localStorage',
		storeAuthStateInCookie: false
	}
};

export const scopes = ['openid', 'profile', v.env.PUBLIC_AZURE_B2C_API_ACCESS_SCOPE];

export const msalReady = writable(false);

export const msalInstance = new PublicClientApplication(msalConfig);

export const activeAccount = writable<{
	account: AccountInfo | null;
	token: string | null;
} | null>(null);

export async function initMsal() {
	console.log('init msal');
	await msalInstance.initialize();
	msalReady.set(true);

	try {
		const response = await msalInstance.handleRedirectPromise();
		const account = response?.account || msalInstance.getAllAccounts()[0];

		if (response) {
			console.log('handling redirect promise');
			// This block runs when returning from a redirect login.
			msalInstance.setActiveAccount(response.account);
			// The access token from the redirect response is fresh.
			activeAccount.set({
				account: response.account,
				token: response.accessToken
			});
			cookieStore.set('auth_token', response.accessToken);
			return;
		}

		if (account) {
			console.log('silent login');
			msalInstance.setActiveAccount(account);
			// This block runs on a page load when the user is already signed in.
			// Silently acquire a token to ensure it's not expired.
			const silentTokenResult = await msalInstance.acquireTokenSilent({
				scopes: scopes,
				account: account
			});

			activeAccount.set({
				account: silentTokenResult.account,
				token: silentTokenResult.accessToken
			});
			cookieStore.set('auth_token', silentTokenResult.accessToken);
		} else {
			// No account found, ensure we are in a signed-out state.
			msalInstance.setActiveAccount(null);
			activeAccount.set(null);
			cookieStore.delete('auth_token');
		}
	} catch (error) {
		if (error instanceof InteractionRequiredAuthError) {
			// Fallback to interactive login if silent acquisition fails
			msalInstance.acquireTokenRedirect({
				scopes: scopes
			});
		} else {
			console.error('MSAL initialization error', error);
			// Handle other errors, perhaps by clearing state
			msalInstance.setActiveAccount(null);
			activeAccount.set(null);
			cookieStore.delete('auth_token');
		}
	}
}

export const signIn = async () => {
	if (msalReady) {
		console.log('sign in');
		await msalInstance.loginRedirect({
			scopes: scopes
		});
	}
};

export const signOut = async () => {
	if (msalReady) {
		await msalInstance.logoutRedirect({
			postLogoutRedirectUri: '/login'
		});
	}
};
