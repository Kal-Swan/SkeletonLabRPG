<script lang="ts">
	let { errors, children } = $props<{ errors: string[]; children: () => any }>();
</script>

{@render children()}

{#if errors && errors.length > 0}
	<div class="text-sm text-red-900/50">
		{#each errors as error}
			<p>{error}</p>
		{/each}
	</div>
{/if}
