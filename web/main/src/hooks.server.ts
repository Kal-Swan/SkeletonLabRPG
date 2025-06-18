// src/hooks.server.ts
import { jwtVerify, createRemoteJWKSet } from 'jose';
import { redirect, type Handle } from '@sveltejs/kit';

const tenantName = 'skeletonlabrpg.ciamlogin.com';
const clientId = 'de827b2c-7ddd-4903-8bd8-43d6315cdeab';
const tenantId = 'a293616a-8cfa-4f4e-9572-41d21ec06a05';
const issuer = `https://${tenantId}.ciamlogin.com/${tenantId}/v2.0`;
const jwksUri = `https://login.microsoftonline.com/${tenantId}/discovery/v2.0/keys`;

const JWKS = createRemoteJWKSet(new URL(jwksUri));

export const handle: Handle = async ({ event, resolve }) => {
	console.log('hooks');
	const token = event.cookies.get('auth_token');

	console.log('token', token);

	if (token) {
		try {
			const { payload } = await jwtVerify(token, JWKS, {
				issuer,
				audience: clientId
			});

			console.log('hooks');
			console.log(payload);

			event.locals.token = token;
		} catch (err) {
			console.error('JWT verification failed', err);
			event.locals.token = null;
		}
	} else {
		event.locals.token = null;
	}

	if (event.url.pathname.startsWith('/.well-known/appspecific/com.chrome.devtools')) {
		return new Response(null, { status: 204 }); // Return empty response with 204 No Content
	}

	return resolve(event);
};
