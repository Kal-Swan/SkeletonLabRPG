<script lang="ts">
	import { signIn } from '@lib/auth/msal-client';
</script>

<div class="m-2">
	<div class="flex w-full justify-between border-b-1 border-b-white p-3">
		<div>Skeleton Lab RPG</div>
		<button class="cursor-pointer" onclick={signIn}>Sign in</button>
	</div>
	<div class="mt-2">
		<div class="title mb-5 w-full text-lg italic">What is the purpose of Skeleton Lab RPG?</div>
		<div class="mb-2">Select your RPG system e.g. Daggerheart, BG3 etc</div>
		<div class="mb-2">
			Ask AI a question on suitable builds for the selected system e.g. "What class does the
			strongest physical damage". Currently using training model GPT-4.
		</div>
		<div class="mb-2">Choose favourte build, edit builds, save builds etc</div>
		<div class="mb-2">More features to come.</div>
	</div>
</div>

<style>
	.title {
		color: var(--secondary);
	}
</style>
