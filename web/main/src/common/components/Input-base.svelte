<script lang="ts">
	let { data, children } = $props();
</script>

<div>
	{@render children()}
	{#if data !== null && data.status === 400}
		<div class="parent flex flex-col">
			{#each data.response as error}
				<div class="text-sm">
					- {error}
				</div>
			{/each}
		</div>
	{/if}
</div>

<style>
	.parent {
		background-color: rgba(255, 0, 0, 0.5);
		color: white;
		padding: 10px;
		margin: 10px;
		font-size: small;
	}
</style>
