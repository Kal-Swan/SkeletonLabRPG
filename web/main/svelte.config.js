import adapter from '@sveltejs/adapter-node';
import { vitePreprocess } from '@sveltejs/vite-plugin-svelte';

const config = {
	preprocess: vitePreprocess(),
	kit: {
		adapter: adapter(),
		alias: {
			'@models/*': 'src/common/models/*',
			'@components/*': 'src/common/components/*',
			'@helpers/*': 'src/common/helpers/*',
			'@environment/*': 'src/common/environment/*',
			'@lib/*': 'src/lib/*',
			'@actions/*': 'src/actions/*'
		}
	}
};

export default config;
