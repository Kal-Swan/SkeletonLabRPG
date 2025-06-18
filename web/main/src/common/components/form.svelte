<script lang="ts" generics="T extends ZodType">
	import type { z, ZodType } from 'zod';
	import TextButton from './text-button.svelte';
	import { setContext } from 'svelte';
	import { writable, type Writable } from 'svelte/store';
	import { zodFieldErrorExtraction } from '@helpers/zod-error-extraction';
	import Loading from './loading.svelte';

	let {
		schema,
		handleSave,
		data,
		children,
		close,
		loading
	}: {
		schema: T;
		handleSave: (data: z.infer<T>) => void;
		data: z.infer<T>;
		children: () => any;
		close?: () => void;
		loading?: boolean;
	} = $props();

	const FORM_ERRORS_CONTEXT_KEY = 'FormErrorsContext';
	const formErrorsStore: Writable<
		{
			field: string;
			message: string;
		}[]
	> = writable([]);
	setContext(FORM_ERRORS_CONTEXT_KEY, formErrorsStore);

	async function handleValidationOnSave() {
		console.log('save clicked');
		const parsed = schema.safeParse(data);
		if (parsed.success) {
			console.log('Validation successful', parsed.data);
			formErrorsStore.set([]);
			await handleSave(parsed.data);
		} else {
			console.error('Validation failed', parsed.error);
			formErrorsStore.set(zodFieldErrorExtraction(parsed.error));
		}
	}
	let isDataValid = $derived(schema.safeParse(data).success);
</script>

<div class="space-y-4">
	{@render children()}
	<div class="flex justify-end gap-2">
		{#if close}
			<TextButton text="Close" onClick={close} />
		{/if}
		<div class="flex">
			{#if loading}
				<Loading />
			{:else}
				<TextButton disable={!isDataValid} text="Save" onClick={handleValidationOnSave} />
			{/if}
		</div>
	</div>
</div>
