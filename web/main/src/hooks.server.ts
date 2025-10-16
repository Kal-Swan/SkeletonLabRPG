// src/hooks.server.ts
import { jwtVerify, createRemoteJWKSet } from 'jose';
import { type Handle } from '@sveltejs/kit';
import * as v from '$env/dynamic/public';
// import { getB2CSettings } from '@lib/auth/configuration';
// import { configStore } from '@lib/stores/config-store';
console.log('hooks');
// const settings = await getB2CSettings();

const tenantName = v.env.PUBLIC_AZURE_B2C_TENANT;
const apiClientId = v.env.PUBLIC_AZURE_B2C_API_CLIENT_ID;
const tenantId = v.env.PUBLIC_AZURE_B2C_TENANT_ID;

const issuer = `https://${tenantId}.ciamlogin.com/${tenantId}/v2.0`;
const jwksUri = `https://${tenantName}/${tenantId}/discovery/v2.0/keys`;

const JWKS = createRemoteJWKSet(new URL(jwksUri));

export const handle: Handle = async ({ event, resolve }) => {
	const token = event.cookies.get('auth_token');

	if (token) {
		try {
			await jwtVerify(token, JWKS, {
				issuer,
				audience: apiClientId
			});

			event.locals.token = token;
			event.locals.isAuthenticated = true;
		} catch (err) {
			event.locals.token = null;
			event.locals.isAuthenticated = false;
		}
	} else {
		event.locals.token = null;
		event.locals.isAuthenticated = false;
	}

	if (event.url.pathname.startsWith('/.well-known/appspecific/com.chrome.devtools')) {
		return new Response(null, { status: 204 }); // Return empty response with 204 No Content
	}

	return resolve(event);
};
