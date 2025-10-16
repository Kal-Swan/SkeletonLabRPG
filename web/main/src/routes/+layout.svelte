<script lang="ts">
	import { accessTokenStore } from '@lib/stores/auth';
	import '../app.css';
	import { configStore } from '@lib/stores/config-store';
	import { get } from 'svelte/store';

	let { children, data } = $props();
	configStore.set(data.config!);
	$effect(() => {
		if ($accessTokenStore) {
			cookieStore.set('auth_token', $accessTokenStore);
		} else {
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
