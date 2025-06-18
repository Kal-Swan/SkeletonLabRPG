<script lang="ts" generics="T">
	import { ButtonType, IconType } from '@models/Icon-type';
	import IconButton from './icon-button.svelte';
	import Icon from '@iconify/svelte';
	import BaseField from './base-field.svelte';

	export let options: Array<{ value: T; label: string }> = [];
	export let value: T;
	export let errors: string[] = [];
</script>

<BaseField {errors}>
	<div class="relative">
		<select bind:value class="unstyled-select border-solid focus:ring-0">
			{#each options as option}
				<option class="unstyled-option border-solid focus:ring-0" value={option.value}
					>{option.label}</option
				>
			{/each}
		</select>
		<!-- <IconButton onClick={() => {}} buttonType={ButtonType.add} iconType={IconType.downArrow} /> -->
		<Icon
			class="pointer-events-none  absolute right-2 top-1/2 -translate-y-1/2"
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

	.unstyled-option {
		cursor: pointer;
		border-color: #6b7280;
		background: transparent;
		background-color: #1e1e2e;
		color: white;
	}
</style>
