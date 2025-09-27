<!-- src/routes/auth/callback/+page.svelte -->
<script lang="ts">
	import { goto } from '$app/navigation';
	import Loading from '@components/loading.svelte';
	import { activeAccount } from '@lib/auth/msal-client';

	$effect(() => {
		// The initMsal in +layout.svelte handles the redirect and sets the activeAccount.
		// We just need to wait for the account to be available and then proceed.
		if ($activeAccount?.token) {
			(async () => {
				try {
					// This fetch can be used to set a secure, httpOnly cookie on the server
					// by passing the token to a server-side endpoint.
					await fetch('/auth/callback', {
						method: 'POST',
						body: JSON.stringify({ token: $activeAccount.token }),
						headers: {
							'Content-Type': 'application/json'
						}
					});
					// Redirect to the main application page after login.
					goto('/rpgsystem', { replaceState: true });
				} catch (error) {
					console.error('Failed to complete post-login actions:', error);
					// Optionally handle this error, e.g., by showing a message to the user.
					// For now, we will still try to redirect.
					goto('/rpgsystem', { replaceState: true });
				}
			})();
		}
	});
</script>

<div class="flex h-screen w-full items-center justify-center text-lg">
	Logging you in&nbsp;<Loading />
</div>
