import { PublicClientApplication } from '@azure/msal-browser';
import * as v from '$env/dynamic/public';

const msalConfig = {
	auth: {
		clientId: v.env.PUBLIC_AZURE_B2C_CLIENT_ID ?? 'fbbfc1ed-5b85-45ed-b470-906c3903f4b4',
		authority: 'https://skeletonlabrpg.ciamlogin.com/a293616a-8cfa-4f4e-9572-41d21ec06a05/v2.0',
		redirectUri: v.env.PUBLIC_AZURE_B2C_REDIRECT_URI ?? 'http://localhost:5173/auth/callback',
		knownAuthorities: ['skeletonlabrpg.ciamlogin.com'],
		navigateToLoginRequestUrl: false
	},
	cache: {
		cacheLocation: 'localStorage',
		storeAuthStateInCookie: false
	}
};

export const msalInstance = new PublicClientApplication(msalConfig);
