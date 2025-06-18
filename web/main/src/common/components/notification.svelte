<script lang="ts">
	import Icon from '@iconify/svelte';
	import { errorMessage } from '@lib/stores/error-message';
	let errors = $state<string[]>([]);
	const onClose = () => {
		errorMessage.set([]);
	};

	errorMessage.subscribe((value) => {
		if (value) {
			errors = value;
		}
	});
</script>

{#if errors.length > 0}
	<div
		class="absolute bottom-0 left-0 right-0 z-10 m-5 flex justify-between rounded-md bg-red-500/15 p-4"
	>
		<div>
			{#each errors as error}
				<p class="text-white">{error}</p>
			{/each}
		</div>
		<Icon
			class="cursor-pointer"
			onclick={onClose}
			icon="solar:close-circle-line-duotone"
			width="24"
			height="24"
		/>
	</div>
{/if}
