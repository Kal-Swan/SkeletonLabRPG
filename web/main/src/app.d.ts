// See https://svelte.dev/docs/kit/types#app.d.ts

import type { AccountInfo } from '@azure/msal-browser';

// for information about these interfaces
declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			token: string | null;
			account: AccountInfo | null;
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
