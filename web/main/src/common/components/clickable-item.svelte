<script lang="ts">
	let { text, subtitle, clicked, handleClick } = $props<{
		text: string;
		subtitle?: string;
		clicked: boolean;
		handleClick: () => void;
	}>();
</script>

<button
	onclick={handleClick}
	class={`input-field cursor-pointer border-solid focus:rounded-sm focus:border-solid focus:ring-0`}
>
	<div>
		{text}
	</div>
	{#if subtitle}
		<div class="text-left text-sm text-gray-500">{subtitle}</div>
	{/if}
</button>

<style>
	.text-field {
		background: transparent;
		caret-color: white;
	}

	.text-field:focus {
		border-color: #6b7280;
	}

	.on-click {
		cursor: pointer;
		background-color: #42425b;
	}
</style>
