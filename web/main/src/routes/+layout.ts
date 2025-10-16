import { getMsalInstance } from '@lib/auth/msal-client';
import { activeAccount, msalReady } from '@lib/stores/auth';
import type { LayoutLoad } from './$types';
import { InteractionRequiredAuthError } from '@azure/msal-browser';
import { accessTokenStore, msalInstanceStore, defaultScopes } from '@lib/stores/auth';

export const load: LayoutLoad = async ({ data }) => {
	const msal = getMsalInstance(data.config);
	msalInstanceStore.set(msal);

	await msal.initialize();
	msalReady.set(true);

	try {
		const response = await msal.handleRedirectPromise();
		const account = response?.account || msal.getAllAccounts()[0];
		if (response) {
			// This block runs when returning from a redirect login.
			msal.setActiveAccount(response.account);
			// The access token from the redirect response is fresh.
			activeAccount.set({
				account: response.account,
				token: response.accessToken
			});
			accessTokenStore.set(response.accessToken);
			// cookieStore.set('auth_token', response.accessToken);
			return;
		}

		if (account) {
			msal.setActiveAccount(account);
			// This block runs on a page load when the user is already signed in.
			// Silently acquire a token to ensure it's not expired.
			const silentTokenResult = await msal.acquireTokenSilent({
				scopes: [...defaultScopes, data.config.b2c.apiAccessScope],
				account: account
			});

			activeAccount.set({
				account: silentTokenResult.account,
				token: silentTokenResult.accessToken
			});
			accessTokenStore.set(silentTokenResult.accessToken);
			// cookieStore.set('auth_token', silentTokenResult.accessToken);
		} else {
			// No account found, ensure we are in a signed-out state.
			msal.setActiveAccount(null);
			activeAccount.set(null);
			accessTokenStore.set(null);
			// cookieStore.delete('auth_token');
		}
	} catch (error) {
		if (error instanceof InteractionRequiredAuthError) {
			// Fallback to interactive login if silent acquisition fails
			msal.acquireTokenRedirect({
				scopes: [...defaultScopes, data.config.b2c.apiAccessScope]
			});
		} else {
			console.error('MSAL initialization error', error);
			// Handle other errors, perhaps by clearing state
			msal.setActiveAccount(null);
			activeAccount.set(null);
			accessTokenStore.set(null);
			// cookieStore.delete('auth_token');
		}
	}

	const signIn = async () => {
		console.log('signIn called');
		if (msalReady) {
			console.log('sign in');
			await msal.loginRedirect({
				scopes: [...defaultScopes, data.config.b2c.apiAccessScope]
			});
		}
	};

	const signOut = async () => {
		console.log('signOut called');
		if (msalReady) {
			await msal.logoutRedirect({
				postLogoutRedirectUri: '/login'
			});
		}
	};

	return { signIn, signOut, config: data.config };
};
