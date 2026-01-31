<script lang="ts">
	import Loading from './loading.svelte';
	let {
		text,
		onClick,
		disableBorder,
		id,
		index,
		disable,
		fontSize = '14',
		padding = '3',
		border = true,
		selected = false,
		loading = false
	} = $props<{
		text: string;
		onClick: (value: any) => void;
		disableBorder?: boolean;
		id?: string;
		index?: number;
		disable?: boolean;
		fontSize?: string;
		padding?: string;
		border?: boolean;
		selected?: boolean;
		loading?: boolean;
	}>();
	let readonly = $state(false);

	async function action(event?: MouseEvent) {
		event?.stopPropagation();
		if (readonly) return;
		console.log('inner button');
		readonly = true;
		try {
			await onClick();
		} finally {
			readonly = false;
		}
	}
</script>

<button
	role="menuitem"
	disabled={disable ?? readonly}
	onclick={action}
	id={`menu-item-${id}-${index}`}
	style={`border: ${border ? '1px solid #6b7280' : 'none'}`}
	class={`${disable ? 'disable-button' : `button-container ${disableBorder ? '' : 'button-container-border'}`}`}
>
	<div
		style={`font-size: ${fontSize}px;`}
		class={`${disable ? `disable-button p-${padding} text-sm` : `button-text-container ${disableBorder ? '' : `p-${padding}`} text-left text-sm`} ${selected ? 'selected' : ''}`}
	>
		{#if loading}
			<Loading />
		{:else}
			{text}
		{/if}
	</div>
</button>

<style>
	.button-container {
		display: inline-block;
		position: relative;
		cursor: pointer;
		background: transparent;
	}

	.button-container-border {
		border-width: 1px;
		transition: border-width 0.3s ease-in-out;
		border: 1px solid #6b7280;
	}

	.disable-button {
		border: 1px solid #4e535e;
		color: #4e535e;
		cursor: not-allowed;
	}

	.button-text-container {
		transition: transform 0.3s ease-in-out;
		transform: scale(1);
	}

	.button-text-container:hover {
		transform: scale(1.1);
		color: var(--secondary);
	}

	.selected {
		transform: scale(1.1);
		color: var(--secondary);
	}
</style>
