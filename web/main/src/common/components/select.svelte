<script lang="ts" generics="T">
	import Icon from '@iconify/svelte';

	let {
		options,
		value = $bindable(),
		isMenu = false
	} = $props<{
		options: Array<{ id: T; name: string }>;
		value: T;
		isMenu?: boolean;
	}>();
	let open = $state(false);
	let selected = $state<T>();

	const onSelect = (id: T) => {
		value = id;
		selected = id;
		open = false;
	};

	$effect(() => {
		const handleClickOutside = (event: MouseEvent) => {
			const parentMenuIcon = document.querySelector('#menu-icon');

			if (
				open &&
				isMenu &&
				event.target !== document.querySelector('.ul-dropdown') &&
				event.target !== parentMenuIcon &&
				!Array.from(parentMenuIcon?.childNodes ?? []).includes(event.target as HTMLElement)
			) {
				open = false;
			}
			if (
				open &&
				!isMenu &&
				event.target !== document.querySelector('.ul-dropdown') &&
				event.target !== document.querySelector('.button-dropdown')
			) {
				console.log('in click evnet');
				console.log(event.target);
				open = false;
			}
		};

		if (open) {
			document.addEventListener('click', handleClickOutside);
		} else {
			document.removeEventListener('click', handleClickOutside);
		}
		return () => document.removeEventListener('click', handleClickOutside);
	});
</script>

<div class="relative w-full">
	{#if isMenu}
		<Icon
			onclick={() => (open = !open)}
			id="menu-icon"
			class="cursor-pointer"
			icon="solar:hamburger-menu-outline"
			width="24"
			height="24"
		/>
	{:else}
		<button
			class="button-dropdown mb-1 flex w-full cursor-pointer justify-between border-1 p-2 align-middle"
			onclick={() => (open = !open)}
		>
			<div>
				{options.find((o: { id: T; name: string }) => o.id === selected)?.name ??
					'Select an option'}
			</div>
			<Icon
				class="self-center"
				icon="solar:round-alt-arrow-down-line-duotone"
				width="14"
				height="14"
			/>
		</button>
	{/if}
	{#if open}
		<ul
			class={`ul-dropdown absolute ${isMenu ? 'right-0 w-40' : 'w-full'} z-10 mt-0 rounded shadow`}
		>
			{#each options as option}
				<li>
					<button
						class="li-dropdown z-10 w-full cursor-pointer px-3 py-2 text-left"
						onclick={() => onSelect(option.id)}>{option.name}</button
					>
				</li>
			{/each}
		</ul>
	{/if}
</div>

<style>
	.button-dropdown {
		border-color: var(--secondary);
	}

	.button-dropdown:hover {
		border-color: white;
	}

	.button-dropdown:focus {
		border-color: white;
	}

	.ul-dropdown {
		background-color: var(--secondary);
	}

	.li-dropdown {
		background-color: var(--secondary);
	}

	.li-dropdown:hover {
		background-color: #a9a9bc;
	}
</style>
