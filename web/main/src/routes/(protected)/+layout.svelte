<script lang="ts">
	import Navigation from '@components/navigation.svelte';
	import Notification from '@components/notification.svelte';
	let { children, data } = $props();
</script>

<div>
	<Navigation signOut={data.signOut} />
	{@render children()}
	<Notification />
</div>
