<!-- src/routes/auth/callback/+page.svelte -->
<script lang="ts">
	import { goto } from '$app/navigation';
	import { msalInstance } from '@lib/auth/msal-client';
	import { msalReady } from '@lib/stores/msal';

	$effect(() => {
		(async () => {
			try {
				msalReady.set(false);
				await msalInstance.initialize();
				let accessToken: string | undefined;
				const result = await msalInstance.handleRedirectPromise();
				if (result) {
					msalInstance.setActiveAccount(result.account);
					accessToken = result.accessToken;
				}
				const account = msalInstance.getAllAccounts()[0];
				if (!accessToken && account) {
					const response = await msalInstance.acquireTokenSilent({
						scopes: ['api://de827b2c-7ddd-4903-8bd8-43d6315cdeab/access'],
						account
					});

					accessToken = response.accessToken;
				}

				await fetch('/auth/callback', {
					method: 'POST',
					body: JSON.stringify({ token: accessToken }),
					headers: {
						'Content-Type': 'application/json'
					}
				});

				goto('/character');
			} catch (error) {
				console.error('Auth error:', error);
			} finally {
				msalReady.set(true);
			}
		})();
	});
</script>

<p>Logging you in...</p>
