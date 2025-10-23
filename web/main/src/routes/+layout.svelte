<script lang="ts">
	import { accessTokenStore } from '@lib/stores/auth';
	import '../app.css';
	import { configStore } from '@lib/stores/config-store';
	import { get } from 'svelte/store';

	let { children, data } = $props();
	console.log('Layout root data:');
	console.log(data);
	configStore.set(data.config!);
	$effect(() => {
		console.log('effect in +layout.svelte - sync access token to cookieStore');
		console.log($accessTokenStore);
		if ($accessTokenStore) {
			console.log('setting auth_token cookie');
			cookieStore.set('auth_token', $accessTokenStore);
		} else {
			console.log('deleting auth_token cookie');
			cookieStore.delete('auth_token');
		}
	});
</script>

<div class="main-container">
	{@render children()}
</div>

<style>
	.main-container {
		margin: 10px;
		position: relative;
	}
</style>
