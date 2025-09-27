<script lang="ts">
	import { goto } from '$app/navigation';
	import TextButton from './text-button.svelte';
	import { page } from '$app/state';
	import { msalInstance, signOut, activeAccount } from '@lib/auth/msal-client';
	import Icon from '@iconify/svelte';
	import Select from './select.svelte';

	let isMobile = $state<boolean>(false);
	let selectedOption = $state<string>('');
	let previousOption = $state<string>('');
	const handleNavigation = (route: string) => {
		goto(route);
	};

	const selectedPath = (path: string) => {
		return page.url.pathname === path;
	};

	const menuOptions = [
		{ id: '/llm', name: 'Ask AI Question' },
		{ id: '/build', name: 'Builds' },
		{ id: '/rpgsystem', name: 'Rpg System' },
		{ id: 'signout', name: 'Sign Out' }
	];

	$effect(() => {
		console.log('navigation');
		console.log($activeAccount?.account?.username);

		if (previousOption !== selectedOption) {
			previousOption = selectedOption;
			if (selectedOption === 'signout') {
				signOut();
			} else if (selectedOption === 'username') {
				// do nothings
			} else if (selectedOption) {
				handleNavigation(selectedOption);
			}
		}

		const media = window.matchMedia('(max-width: 768px)');
		isMobile = media.matches;
		const listener = (e: any) => (isMobile = e.matches);
		media.addEventListener('change', listener);

		return () => media.removeEventListener('change', listener);
	});
</script>

{#if isMobile}
	<div class="relative w-full p-2">
		<div class="flex w-full items-end justify-between">
			<div class="logo p-1">
				<div class=" text-sm">{$activeAccount?.account?.username!}</div>
			</div>
			<div>
				<Select isMenu={true} options={menuOptions} bind:value={selectedOption} />
			</div>
		</div>
	</div>
{:else}
	<div
		class="navigation-container mt-2 mr-2 mb-2 flex w-full justify-between border border-white p-3"
	>
		<div class="logo p-1">Skeleton Lab RPG</div>
		<div>
			<TextButton
				padding="1"
				border={false}
				text="Ask AI Question"
				selected={selectedPath('/llm')}
				onClick={() => handleNavigation('/llm')}
			/>
			<TextButton
				padding="1"
				border={false}
				text="Builds"
				onClick={() => handleNavigation('/build')}
				selected={selectedPath('/build')}
			/>
			<TextButton
				padding="1"
				border={false}
				text="Rpg System"
				selected={selectedPath('/rpgsystem')}
				onClick={() => handleNavigation('/rpgsystem')}
			/>
		</div>
		<div>
			{$activeAccount?.account?.username}
			<TextButton padding="1" border={false} text="Sign Out" onClick={signOut} />
		</div>
	</div>
{/if}

<style>
	.navigation-container {
		background-color: var(--primary);
	}

	.logo {
		color: var(--secondary);
	}

	.mobile-menu {
		background-color: var(--primary);
	}
</style>
