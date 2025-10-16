import { DefaultAzureCredential } from '@azure/identity';
import { AppConfigurationClient } from '@azure/app-configuration';
import * as v from '$env/dynamic/public';
const client = new AppConfigurationClient(
	'https://skeletonlabrpg-shared-appconfig.azconfig.io',
	new DefaultAzureCredential()
);

export const getAzureConfiguration = async (): Promise<Configuration> => {
	const setting = client.listConfigurationSettings({
		keyFilter: '*',
		labelFilter: '*'
	});

	const configuration: Partial<Configuration> = {};
	const b2cSetting: Partial<B2C> = {};
	for await (const item of setting) {
		if (item.key === 'ApiUrl' && (item.label === v.env.PUBLIC_ENV || item.label === undefined)) {
			configuration.apiUrl = item.value;
		}
		if (
			item.key.startsWith('B2C:') &&
			(item.label === v.env.PUBLIC_ENV || item.label === undefined)
		) {
			const key = item.key.replace('B2C:', '');
			const camelCaseKey = key.charAt(0).toLowerCase() + key.slice(1);
			(b2cSetting as any)[camelCaseKey] = item.value;
		}
	}

	configuration.b2c = b2cSetting as B2C;
	return configuration as Configuration;
};

export interface Configuration {
	apiUrl: string;
	b2c: B2C;
}

interface B2C {
	apiAccessScope: string;
	apiClientId: string;
	authority: string;
	redirectUri: string;
	tenant: string;
	tenantId: string;
	webClientId: string;
}
