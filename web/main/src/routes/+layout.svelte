<script lang="ts">
	import Notification from '@components/notification.svelte';
	import '../app.css';
	//import Menu from '@components/menu.svelte';
	let { children } = $props();
</script>

<!-- <Menu /> -->
<Notification />
<div class="main-container">
	{@render children()}
</div>

<style>
	.main-container {
		margin: 10px;
	}
</style>
