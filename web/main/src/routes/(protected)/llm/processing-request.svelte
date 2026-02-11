<script lang="ts">
	import { Tween } from 'svelte/motion';
	import {
		BuildRequestStatus,
		type buildRequestProgressType,
		type buildRequestType
	} from './build-details-schema';
	import { cubicOut } from 'svelte/easing';
	import { hubConnection } from '@lib/signalr/buildhub';
	import { SignalRHubConstants } from './signalr-hub-constants';
	import Label from '@components/label.svelte';

	let { request } = $props<{ request: buildRequestType }>();
	console.log('Processing request:', request);
	const tween = new Tween(request.progression ?? 0, { duration: 400, easing: cubicOut });

	$effect(() => {
		hubConnection.on(SignalRHubConstants.BuildRequestProgress, (data) => {
			const resultBuildRequest = data as buildRequestProgressType;
			if (resultBuildRequest.id === request.id) {
				request.status = resultBuildRequest.status;
				tween.set(resultBuildRequest.progression);
			}
		});
	});
</script>

<Label text={request.buildSystemName} textSize="text-sm" />
<p class="text-sm text-gray-700">Question: {request.question}</p>
<p class="text-sm text-gray-700">
	Status: {request.status === BuildRequestStatus.Processing
		? `Processing (${Math.round(tween.current)}%)`
		: 'Queued'}
</p>
