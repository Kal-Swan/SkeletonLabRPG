// src/hooks.server.ts
import { jwtVerify, createRemoteJWKSet } from 'jose';
import { type Handle } from '@sveltejs/kit';
import * as v from '$env/dynamic/public';
import { configStore } from '@lib/stores/config-store';
import { get } from 'svelte/store';

export const handle: Handle = async ({ event, resolve }) => {
	console.log('hooks');
	console.log(get(configStore));
	const config = get(configStore);

	if (config?.b2c == null) {
		console.log('No config found in store, skipping auth handling');
		return resolve(event);
	}

	const token = event.cookies.get('auth_token');
	console.log('Token from cookies:');
	console.log(token);

	const issuer = `https://${config.b2c.tenantId}.ciamlogin.com/${config.b2c.tenantId}/v2.0`;
	const jwksUri = `https://${config.b2c.tenant}/${config.b2c.tenantId}/discovery/v2.0/keys`;

	const JWKS = createRemoteJWKSet(new URL(jwksUri));

	if (token) {
		console.log('Verifying token');
		try {
			await jwtVerify(token, JWKS, {
				issuer,
				audience: config.b2c.apiClientId
			});
			console.log('Token is valid');

			event.locals.token = token;
			event.locals.isAuthenticated = true;
		} catch (err) {
			console.log('Token verification failed:', err);
			console.error(err);
			event.locals.token = null;
			event.locals.isAuthenticated = false;
		}
	} else {
		console.log('No token found in cookies');
		event.locals.token = null;
		event.locals.isAuthenticated = false;
	}

	if (event.url.pathname.startsWith('/.well-known/appspecific/com.chrome.devtools')) {
		return new Response(null, { status: 204 }); // Return empty response with 204 No Content
	}

	return resolve(event);
};
