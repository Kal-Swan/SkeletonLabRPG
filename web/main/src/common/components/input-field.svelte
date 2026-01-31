<script lang="ts">
	import type { HTMLInputTypeAttribute } from 'svelte/elements';
	import BaseField from './base-field.svelte';
	import { getContext } from 'svelte';
	import type { Readable } from 'svelte/store';
	import Label from './label.svelte';
	import Textarea from './textarea.svelte';

	let {
		name,
		value = $bindable(null),
		placeholder,
		type,
		readonly,
		checked = $bindable(false),
		underline,
		isTextArea,
		onChange,
		className,
		inputFieldId = 'file'
	} = $props<{
		value?: string | number | boolean | null | unknown;
		placeholder?: string;
		type?: HTMLInputTypeAttribute | null | undefined;
		readonly?: boolean;
		checked?: boolean;
		underline?: boolean;
		errors?: string[];
		name?: string;
		isTextArea?: boolean;
		className?: string;
		onChange?: (event: Event) => void;
		inputFieldId?: string;
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
	<Label text={name} textSize="text-sm" />
	{#if type === 'checkbox'}
		<input
			class="{underline ? 'underline' : ''} checkbox-field focus:ring-0"
			type="checkbox"
			{placeholder}
			bind:checked
			{readonly}
		/>
	{:else if isTextArea}
		<Textarea classNames="input-field" minRows={1} maxRows={10} bind:value></Textarea>
	{:else if type === 'file'}
		<!-- <input
			class="file-field hover:cursor-pointer focus:ring-0"
			{type}
			onchange={onChange}
			{placeholder}
			bind:value
			{readonly}
		/> -->
		<input onchange={onChange} id={inputFieldId} type="file" class="hidden" />
		<label
			for={inputFieldId}
			class="file-field file-field-text-container flex cursor-pointer items-center justify-center text-white"
		>
			Upload File
		</label>
	{:else}
		<input
			class="input-field focus:ring-0 {type === 'file' ? 'hover:cursor-pointer' : ''} {className
				? className
				: ''}"
			{type}
			onchange={onChange}
			{placeholder}
			bind:value
			{readonly}
		/>
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

	.file-field-text-container {
		transition: font-size 0.3s ease-in-out;
		font-size: 14px;
		border: 1px solid var(--secondary);
		width: 150px;
		height: 40px;
	}

	.file-field-text-container:hover {
		font-size: 15px;
		color: var(--secondary);
	}
</style>
