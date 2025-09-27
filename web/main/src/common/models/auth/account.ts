import type { AccountInfo } from '@azure/msal-browser';

export interface ActiveAccount {
	account?: AccountInfo;
	token?: string;
}
