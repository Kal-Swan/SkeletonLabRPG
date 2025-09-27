<script lang="ts">
	import RecursiveItems from './recursive-items.svelte';
	import InputField from './input-field.svelte';

	let { build = $bindable() } = $props<{
		build: Record<string, any>;
	}>();
</script>

{#each Object.entries(build) as [key, value] (key)}
	{#if Array.isArray(value)}
		<RecursiveItems bind:build={build[key]} />
	{:else if typeof value === 'object' && value !== null}
		<RecursiveItems bind:build={build[key]} />
	{:else if key !== 'id'}
		<InputField isTextArea={true} name={key} bind:value={build[key]} />
	{/if}
{/each}
