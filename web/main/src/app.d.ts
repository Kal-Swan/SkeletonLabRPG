// See https://svelte.dev/docs/kit/types#app.d.ts

import type { AccountInfo } from '@azure/msal-browser';
import type { Configuration } from '@lib/configuration';

// for information about these interfaces
declare global {
	namespace App {
		// interface Error {}
		interface Locals {
			token: string | null;
			isAuthenticated: boolean;
			account: AccountInfo | null;
			config: Configuration;
			signIn: () => Promise<void>;
			signOut: () => Promise<void>;
		}
		// interface PageData {}
		// interface PageState {}
		// interface Platform {}
	}
}

export {};
