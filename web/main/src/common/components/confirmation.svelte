<script lang="ts">
	import Icon from '@iconify/svelte';
	import TextButton from './text-button.svelte';
	import type { ActionType } from '@models/actions';
	import { fade } from 'svelte/transition';
	import Loading from './loading.svelte';

	let {
		title,
		message,
		id,
		button,
		open = $bindable(),
		children,
		loading
	} = $props<{
		title: string;
		message: string;
		id: string;
		open: string;
		loading?: boolean;
		button?: {
			text: string;
			onClick: () => void;
			actionType: ActionType;
		};
		children?: () => any;
	}>();

	function onClose() {
		open = '';
		console.log('Confirmation dialog closed: ' + open);
	}
</script>

{#if open.length > 0}
	<div transition:fade class="background-screen fixed inset-0 z-50">
		<div
			class="background-color fixed top-1/2 left-1/2 z-10 flex w-11/12 max-w-md -translate-x-1/2 -translate-y-1/2 flex-col gap-2 rounded-lg border-2 border-amber-50 p-5 shadow-xl"
		>
			<div class="flex flex-row justify-between">
				<div>{title}</div>
				<!-- <div>
					<Icon
						class="cursor-pointer"
						onclick={onClose}
						icon="solar:close-circle-line-duotone"
						width="24"
						height="24"
					/>
				</div> -->
			</div>
			<div>
				{message}
			</div>
			{#if children}
				<div>
					{@render children()}
				</div>
			{/if}
			{#if button}
				<div class="flex justify-end gap-2">
					{#if loading}
						<Loading />
					{:else}
						<TextButton text={button.text} onClick={button.onClick} />
						<TextButton text="Cancel" onClick={onClose} />
					{/if}
				</div>
			{/if}
		</div>
	</div>
{/if}

<style>
	.background-color {
		background-color: #1e1e2e;
	}

	.background-screen {
		background-color: rgba(30, 30, 46, 0.5);
	}
</style>
