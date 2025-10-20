import { PublicClientApplication } from '@azure/msal-browser';
import type { Configuration } from '@lib/configuration';
let msalInstance: PublicClientApplication | null = null;
export function getMsalInstance(config: Configuration) {
	if (!msalInstance) {
		console.log('Creating new MSAL instance');
		msalInstance = new PublicClientApplication({
			auth: {
				clientId: config.b2c.webClientId,
				authority: config.b2c.authority,
				redirectUri: config.b2c.redirectUri,
				knownAuthorities: [config.b2c.tenant],
				navigateToLoginRequestUrl: false
			},
			cache: {
				cacheLocation: 'localStorage',
				storeAuthStateInCookie: config.b2c.storeAuthStateInCookie
			}
		});
	}

	return msalInstance;
}
