<script lang="ts">
	let {
		value = $bindable(),
		minRows,
		maxRows,
		classNames
	} = $props<{
		value: string;
		minRows: number;
		maxRows: number;
		classNames: string;
	}>();

	let minHeight = $derived(`${1 + minRows * 1.2}em`);
	// let maxHeight = $derived(maxRows ? `${1 + maxRows * 1.2}em` : `auto`);
</script>

<div class={'container'}>
	<pre aria-hidden="true" style="min-height: {minHeight}; height: 100%;">{value + '\n'}</pre>
	<textarea bind:value></textarea>
</div>

<style>
	.container {
		position: relative;
		height: 100%;
	}

	pre,
	textarea {
		font-family: inherit;
		padding: 0.5em;
		box-sizing: border-box;
		border-color: #6b7280;
		line-height: 1.2;
		background: #1e1e2e;
	}

	pre {
		white-space: pre-wrap;
		word-wrap: break-word;
	}

	textarea {
		position: absolute;
		width: 100%;
		height: 100%;
		top: 0;
		resize: none;
		overflow: hidden;
	}
</style>
