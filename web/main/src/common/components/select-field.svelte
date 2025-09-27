<script lang="ts" generics="T">
	import { ButtonType, IconType } from '@models/Icon-type';
	import IconButton from './icon-button.svelte';
	import Icon from '@iconify/svelte';
	import BaseField from './base-field.svelte';

	export let options: Array<{ id: T; name: string }> = [];
	export let id: T;
	export let errors: string[] = [];
</script>

<BaseField {errors}>
	<div class="relative">
		<select bind:value={id} class="unstyled-select focus:ring-0">
			{#each options as option}
				<option class="unstyled-option focus:ring-0" value={option.id}>{option.name}</option>
			{/each}
		</select>
		<!-- <IconButton onClick={() => {}} buttonType={ButtonType.add} iconType={IconType.downArrow} /> -->
		<Icon
			class="pointer-events-none  absolute top-1/2 right-2 -translate-y-1/2"
			icon="solar:round-alt-arrow-down-line-duotone"
			width="14"
			height="14"
		/>
	</div>
</BaseField>

<style>
	.unstyled-select {
		width: 100%;
		cursor: pointer;
		background: transparent;
		border-color: #6b7280;
	}

	.unstyled-select:hover {
		border-color: white;
	}

	.unstyled-option {
		cursor: pointer;
		border-color: #6b7280;
		background: transparent;
		background-color: #1e1e2e;
		color: white;
	}

	.unstyled-option:hover {
		background-color: var(--primary);
		color: var(--secondary);
	}
</style>
