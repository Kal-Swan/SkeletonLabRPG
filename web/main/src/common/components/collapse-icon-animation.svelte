<script>
	import Icon from '@iconify/svelte';

	let { collapse } = $props();

	$effect(() => {
		console.log('collapse');
		console.log(collapse);
	});
</script>

<div class:collapsed={collapse} class:uncollapsed={!collapse} class="collapsable-container">
	<Icon icon="solar:alt-arrow-down-line-duotone" width="24" height="24" />
</div>

<style>
	.collapsable-container {
		transition: transform 0.4s ease;
	}

	.uncollapsed {
		transform: rotate(180deg);
	}

	.collapsed {
		transform: rotate(0deg);
	}
</style>
