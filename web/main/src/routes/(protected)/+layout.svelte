<script lang="ts">
	import Navigation from '@components/navigation.svelte';
	import Notification from '@components/notification.svelte';
	let { children } = $props();
</script>

<div>
	<Navigation />
	{@render children()}
	<Notification />
</div>
