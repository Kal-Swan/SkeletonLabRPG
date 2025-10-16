import { getAzureConfiguration } from '@lib/configuration';
import type { LayoutServerLoad } from './$types';
import { configStore } from '@lib/stores/config-store';

export const load: LayoutServerLoad = async ({ locals }) => {
	const config = await getAzureConfiguration();
	console.log('Layout load config');
	console.log(config);
	configStore.set(config);
	return {
		token: locals.token,
		config: config
	};
};
