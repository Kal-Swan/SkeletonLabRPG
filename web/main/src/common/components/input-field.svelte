<script lang="ts">
	import type { HTMLInputTypeAttribute } from 'svelte/elements';
	import BaseField from './base-field.svelte';
	import { getContext } from 'svelte';
	import type { Readable } from 'svelte/store';

	let {
		name,
		value = $bindable(null),
		placeholder,
		type,
		readonly,
		checked = $bindable(false),
		underline
	} = $props<{
		value?: string | number | boolean | null;
		placeholder?: string;
		type?: HTMLInputTypeAttribute | null | undefined;
		readonly?: boolean;
		checked?: boolean;
		underline?: boolean;
		errors?: string[];
		name: string;
	}>();

	const FORM_ERRORS_CONTEXT_KEY = 'FormErrorsContext';
	const formErrorsStore = getContext<
		Readable<
			{
				field: string;
				message: string;
			}[]
		>
	>(FORM_ERRORS_CONTEXT_KEY);

	let fieldErrors = $state<string[]>([]);

	$effect(() => {
		if (formErrorsStore) {
			const unsubscribe = formErrorsStore.subscribe((allErrors) => {
				fieldErrors = allErrors
					.filter((error) => error.field === name)
					.map((error) => error.message);
			});
			return unsubscribe;
		}
	});
</script>

<BaseField errors={fieldErrors}>
	{#if type === 'checkbox'}
		<input
			class="{underline ? 'underline' : ''} checkbox-field focus:ring-0"
			type="checkbox"
			{placeholder}
			bind:checked
			{readonly}
		/>
	{:else}
		<input class="input-field focus:ring-0" {type} {placeholder} bind:value {readonly} />
	{/if}

	{#if type !== 'checkbox' && underline}
		<div class="underline"></div>
	{/if}
</BaseField>

<style>
	.input-field {
		background: transparent;
		caret-color: white;
		width: 100%;
		overflow: hidden;
	}

	.checkbox-field {
		background: transparent;
		caret-color: white;
	}

	.underline {
		border-bottom: 0.5px solid white;
	}

	.input-field:focus {
		border-color: #6b7280;
	}

	.input-field::-webkit-inner-spin-button,
	.input-field::-webkit-outer-spin-button {
		-webkit-appearance: none;
		margin: 0;
	}

	/* .input-field:focus {
		border-color: transparent;
	} */

	.input-field:-webkit-autofill,
	.input-field:-webkit-autofill:hover,
	.input-field:-webkit-autofill:focus,
	.input-field:-webkit-autofill:active {
		-webkit-box-shadow: 0 0 0px 1000px #1e1e2e inset !important;
		-webkit-text-fill-color: white !important; /* Ensure text color is visible */
	}
</style>
