<script lang="ts">
	import Loading from './loading.svelte';

	let { text, onClick, disableBorder, id, index, disable } = $props<{
		text: string;
		onClick: () => void;
		disableBorder?: boolean;
		id?: string;
		index?: number;
		disable?: boolean;
	}>();
	let readonly = $state(false);

	async function action() {
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
	class={`${disable ? 'disable-button' : `button-container ${disableBorder ? '' : 'button-container-border'}`}`}
>
	<div
		class={`${disable ? 'disable-button p-3 text-sm' : `button-text-container ${disableBorder ? '' : 'p-3'} text-left text-sm`}`}
	>
		{text}
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

	/* .button-container:active {
		border-width: 3px;
	} */

	.button-text-container {
		transition: transform 0.3s ease-in-out;
		transform: scale(1);
	}

	.button-text-container:hover {
		transform: scale(1.1);
		color: #7f7f98;
	}
</style>
