<script lang="ts">
	import Icon from '@iconify/svelte';
	import { ButtonType } from '@models/Icon-type';
	export let iconType: string;
	export let buttonType: ButtonType;
	export let onClick: () => void;
</script>

<div class="button-container {buttonType}">
	<Icon onclick={onClick} style="color: white" icon="solar:{iconType}" width="25" height="25" />
</div>

<style>
	.button-container {
		display: inline-block;
		position: relative;
		cursor: pointer;
	}

	.button-container :global(svg path) {
		transition:
			transform 0.3s ease-in-out,
			stroke-width 0.3s ease-in-out;
		transform-origin: center;
	}

	.button-container :global(svg path:nth-child(1)) {
		stroke-width: 1;
	}

	.button-container:active :global(svg path:nth-child(1)) {
		stroke-width: 3;
	}

	.button-container.edit:hover :global(svg path:nth-child(3)) {
		transform: translate(-20%, -40%) scale(1.5);
	}

	.button-container.edit:not(:hover) :global(svg path:nth-child(3)) {
		transform: translate(0, 0) scale(1);
	}

	.button-container.add:hover :global(svg path:nth-child(2)),
	.button-container.delete:hover :global(svg path:nth-child(2)),
	.button-container.cancel:hover :global(svg path:nth-child(2)) {
		transform: scale(1.5);
	}

	.button-container.add:not(:hover) :global(svg path:nth-child(2)),
	.button-container.delete:not(:hover) :global(svg path:nth-child(2)),
	.button-container.cancel:not(:hover) :global(svg path:nth-child(2)) {
		transform: scale(1);
	}
</style>
