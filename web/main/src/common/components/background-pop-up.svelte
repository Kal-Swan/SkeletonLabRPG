<script lang="ts">
	import { fade } from 'svelte/transition';
	let { children } = $props<{ children: () => any }>();
</script>

<div
	transition:fade
	class="background-screen background-color absolute left-1/2 z-50 w-1/2 -translate-x-1/2"
>
	<div
		class="background-color z-10 flex flex-col gap-2 rounded-lg border-2 border-amber-50 p-5 shadow-xl"
	>
		{@render children()}
	</div>
</div>

<style>
	.background-color {
		background-color: #1e1e2e;
	}
</style>
