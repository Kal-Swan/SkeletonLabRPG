<script lang="ts">
</script>

<div class="flex items-center justify-center gap-1">
	<div class="loading-dot">o</div>
	<div class="loading-dot">o</div>
	<div class="loading-dot">o</div>
</div>

<style>
	.loading-dot {
		animation: bounce 0.4s infinite alternate;
	}

	.loading-dot:nth-child(2) {
		animation-delay: 0.2s;
	}

	.loading-dot:nth-child(3) {
		animation-delay: 0.3s;
	}

	@keyframes bounce {
		from {
			transform: translateY(0);
		}
		to {
			transform: translateY(-5px);
		}
	}
</style>
