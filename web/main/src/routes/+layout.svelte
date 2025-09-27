<script lang="ts">
	import '../app.css';
	import { initMsal } from '@lib/auth/msal-client';
	let { children } = $props();

	$effect(() => {
		(async () => initMsal())();
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
