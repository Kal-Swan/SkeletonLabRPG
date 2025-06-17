<script lang="ts">
	import Icon from '@iconify/svelte';
	import TextButton from './text-button.svelte';
	import { clickOutside } from '@actions/clickOutside';

	let { title, id, children, menu, handleItemClick } = $props<{
		title: string;
		id: string;
		menu: { text: string; onClick: (text: string) => void }[];
		handleItemClick?: (id: string) => void;
		children?: () => void;
	}>();

	let menuOpen = $state(false);
	let menuTriggerElement: HTMLButtonElement | undefined = $state();

	function onItemClick() {
		handleItemClick(id);
	}
	function handleItemKeyDown(event: KeyboardEvent) {
		if (event.key === 'Enter' || event.key === ' ') {
			event.preventDefault();
			menuOpen = !menuOpen;
		} else if (event.key === 'Escape' && menuOpen) {
			event.preventDefault();
			menuOpen = false;
			menuTriggerElement?.focus();
		}
	}

	function handleMenuOptionClick(optionOnClick: (text: string) => void, text: string) {
		optionOnClick(text);
		menuOpen = false;
	}

	function menuClick() {
		menuOpen = !menuOpen;
	}

	let menuTriggerButtonId = `${id}-menu-button`;
</script>

<div
	role="button"
	aria-labelledby={`item-title-${id}`}
	onkeydown={handleItemKeyDown}
	tabindex="0"
	onclick={onItemClick}
	class="flex w-full flex-col gap-2 rounded-2xl border p-2 md:w-auto md:min-w-md"
>
	<div class="flex items-center justify-between">
		<div id={`item-title-${id}`} class="font-bold uppercase">{title}</div>
		<div class="relative">
			<button
				id={menuTriggerButtonId}
				onclick={menuClick}
				type="button"
				class="dot rounded-full p-1 focus:outline-none"
			>
				<Icon id={menuTriggerButtonId} icon="solar:menu-dots-bold" width="24" height="24" />
			</button>
			{#if menuOpen}
				<div
					role="menu"
					use:clickOutside={{ callback: menuClick, triggerButtonId: menuTriggerButtonId }}
					id={`menu-dropdown-${id}`}
					class="background-color absolute right-0 z-10 mt-1 flex w-40 origin-top-right flex-col rounded-2xl p-4 text-9xl shadow-lg focus:outline-none"
				>
					{#each menu as item, index (item.text)}
						<TextButton
							id={item.id}
							{index}
							disableBorder
							text={item.text}
							onClick={() => handleMenuOptionClick(item.onClick, item.text)}
						/>
					{/each}
				</div>
			{/if}
		</div>
	</div>
	{@render children()}
</div>

<style>
	.background-color {
		background-color: #1e1e2e;
	}

	.dot {
		border: 1.5px solid transparent;
		transition:
			transform 0.3s ease-in-out,
			border-color 0.3s ease-in-out;
	}

	.dot:hover {
		/* border: 1.5px solid #7f7f98; */
		border-color: #7f7f98;
	}

	.dot:focus {
		border: 1.5px solid #7f7f98;
	}
</style>
