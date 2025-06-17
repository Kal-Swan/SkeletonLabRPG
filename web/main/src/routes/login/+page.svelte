<script lang="ts">
	import { msalInstance } from '@lib/auth/msal-client';
	import { msalReady } from '@lib/stores/msal';

	$effect(() => {
		(async () => {
			msalReady.set(false);
			await msalInstance.initialize();
			try {
				await msalInstance.handleRedirectPromise();
			} catch (err) {
				console.error('Redirect handling error', err);
			} finally {
				msalReady.set(true);
			}
		})();
	});

	const login = async () => {
		console.log('login clicked');
		if ($msalReady) {
			await msalInstance.loginRedirect({
				scopes: ['openid', 'profile', 'api://de827b2c-7ddd-4903-8bd8-43d6315cdeab/access']
			});
		}
	};

	const loginOut = async () => {
		if ($msalReady) {
			await msalInstance.logoutRedirect();
		}
	};
</script>

<div>
	<button class="cursor-pointer" onclick={login}>Sign in with Azure</button>
</div>
<div>
	<button class="cursor-pointer" onclick={loginOut}>Sign out</button>
</div>
