<script lang="ts">
	import Label from '@components/label.svelte';
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';
	import { clientFetch } from '@helpers/client-fetch';
	import { Actions } from './actions';
	import ItemDescription from '@components/item-description.svelte';
	import { rpgBuildQuestionSchema } from '@models/rpgbuild/llm-schema';
	import Select from '@components/select.svelte';
	import { hubConnection } from '@lib/signalr/buildhub';
	import {
		BuildAnswerStatus,
		buildRequestAnswer,
		BuildRequestStatus,
		type buildRequestAnswerType,
		type buildRequestProgressType,
		type buildRequestType,
		type buildSystemType
	} from './build-details-schema.js';
	import { SignalRHubConstants } from './signalr-hub-constants';
	import { Tween } from 'svelte/motion';
	import ProcessingRequest from './processing-request.svelte';
	import CollapseIconAnimation from '@components/collapse-icon-animation.svelte';
	import { slide } from 'svelte/transition';
	import { sineIn } from 'svelte/easing';
	let { data } = $props();
	let {
		buildSystems,
		buildRequests
	}: { buildSystems: buildSystemType[]; buildRequests: buildRequestType[] } = data;
	let loadingAnswer = $state(false);
	let loadingBuildSave = $state<string>('');
	let currentBuildRequests = $state<buildRequestType[]>(
		buildRequests.filter((request) => request.status === BuildRequestStatus.Completed) || []
	);
	let currentProcessingRequests = $state<buildRequestType[]>(
		buildRequests.filter(
			(request) =>
				request.status === BuildRequestStatus.Processing ||
				request.status === BuildRequestStatus.Queued
		) || []
	);
	let rpgBuildQuestion = $state({
		question: '',
		buildSystemId: ''
	});

	let selectedItem = $state<buildRequestAnswerType | null>(null);

	let collapsedQuestions = $state<string[]>([]);

	const menu = [
		{
			text: 'Edit',
			onClick: (text: string, item: buildRequestAnswerType) => {
				selectedItem = item;
			}
		},
		{
			text: 'Delete',
			onClick: async (text: string, item: buildRequestAnswerType) => {
				item.status = BuildAnswerStatus.Deleted;
				const result = await clientFetch<buildRequestType>(
					'/llm',
					Actions.updateBuildRequest,
					item
				);

				if (result.isSuccess) {
					loadingBuildSave = '';

					if (result.data?.answers.every((answer) => answer.status !== BuildAnswerStatus.None)) {
						currentBuildRequests = currentBuildRequests.filter(
							(request) => request.id !== result.data?.id
						);
						return;
					}

					currentBuildRequests = currentBuildRequests.filter(
						(request) => request.id !== result.data?.id
					);
					currentBuildRequests = [...currentBuildRequests, result.data!];
				}
			}
		},
		{
			text: 'Save',
			onClick: async (text: string, item: buildRequestAnswerType) => {
				loadingBuildSave = item.id;
				item.status = BuildAnswerStatus.Saved;

				const result = await clientFetch<buildRequestType>(
					'/llm',
					Actions.updateBuildRequest,
					item
				);

				if (result.isSuccess) {
					loadingBuildSave = '';

					if (result.data?.answers.every((answer) => answer.status !== BuildAnswerStatus.None)) {
						currentBuildRequests = currentBuildRequests.filter(
							(request) => request.id !== result.data?.id
						);
						return;
					}

					currentBuildRequests = currentBuildRequests.filter(
						(request) => request.id !== result.data?.id
					);
					currentBuildRequests = [...currentBuildRequests, result.data!];
				}
			}
		}
	];

	const onBuildItemClick = (item: buildRequestAnswerType) => {
		selectedItem = item;
	};

	const handleItemUpdate = (item: buildRequestAnswerType) => {
		currentBuildRequests = currentBuildRequests.map((build) => {
			if (build.id !== item.buildRequestId) return build;

			return {
				...build,
				answers: build.answers.map((answer) => (answer.id === item.id ? item : answer))
			};
		});

		selectedItem = null;
	};

	const handleItemCancel = () => {
		selectedItem = null;
	};

	async function handleCreateRpgBuildSave() {
		loadingAnswer = true;
		console.log('Submitting question:', rpgBuildQuestion);
		console.log(buildSystems.find((system) => system.id === rpgBuildQuestion.buildSystemId)!.id);
		const result = await clientFetch<buildRequestType>('/llm', Actions.createBuildRequest, {
			question: rpgBuildQuestion.question,
			buildSystemId: buildSystems.find((system) => system.id === rpgBuildQuestion.buildSystemId)!.id
		});

		if (result.isSuccess) {
			loadingAnswer = false;
			currentProcessingRequests = [...currentProcessingRequests, result.data!];
		}
	}

	$effect(() => {
		async function buildRequestListener() {
			await hubConnection.start();

			hubConnection.on(SignalRHubConstants.BuildRequestComplete, (data) => {
				loadingAnswer = false;
				const resultBuildRequest = data as buildRequestType;
				currentBuildRequests = [...currentBuildRequests, resultBuildRequest];
				currentProcessingRequests = currentProcessingRequests.filter(
					(request) => request.id !== resultBuildRequest.id
				);
			});
		}
		buildRequestListener();
	});

	const mapToBuildRequestAnswerType = (
		data: any,
		buildRequestId: string
	): buildRequestAnswerType => {
		return {
			id: data.id,
			name: data.name,
			template: data.template,
			reason: data.reason,
			buildRequestId: buildRequestId,
			status: data.status
		};
	};

	$effect(() => {
		console.log('currentBuildRequests changed: ');
		console.log(currentBuildRequests);
	});

	let sortedBuildRequests = $derived(
		[...currentBuildRequests].sort((a, b) =>
			a.latestProcessedDate > b.latestProcessedDate ? -1 : 1
		)
	);

	function handleCollapsableQuestion(id: string) {
		const questionExists = collapsedQuestions.some((q) => q == id);

		if (questionExists) {
			collapsedQuestions = collapsedQuestions.filter((q) => q != id);
			return;
		}

		collapsedQuestions = [...collapsedQuestions, id];
	}
</script>

<div class="mt-4">
	<Form
		data={rpgBuildQuestion}
		schema={rpgBuildQuestionSchema}
		handleSave={handleCreateRpgBuildSave}
		saveButtonText="Send"
		loading={loadingAnswer}
	>
		<div class="flex flex-col justify-between gap-2 md:flex-row">
			<div class="flex flex-col gap-2 md:flex-[1]">
				<Label text="Choose RPG Build" textSize="text-sm" />
				<Select options={buildSystems} bind:value={rpgBuildQuestion.buildSystemId} />
			</div>

			<div class="flex flex-col gap-2 md:flex-[3]">
				<InputField
					name={'Question'}
					underline={false}
					type="text"
					placeholder="Enter question"
					bind:value={rpgBuildQuestion.question}
				/>
			</div>
		</div>
	</Form>
</div>

{#if currentProcessingRequests.some((request) => request.status === BuildRequestStatus.Processing || request.status === BuildRequestStatus.Queued)}
	<div class="mb-4">
		<Label text="Processing Requests" textSize="text-lg" />
		{#each currentProcessingRequests
			.filter((request) => request.status === BuildRequestStatus.Processing || request.status === BuildRequestStatus.Queued)
			.sort((a, b) => a.buildSystemName.localeCompare(b.buildSystemName)) as request}
			<ProcessingRequest {request} />
		{/each}
	</div>
{/if}

{#if sortedBuildRequests.length > 0}
	<div class="mb-4">
		<Label text="Suggested RPG Builds" textSize="text-lg" />
	</div>
	{#if !selectedItem}
		{#each sortedBuildRequests as buildRequest, index}
			<div class="mt-4 mb-4 gap-2">
				{#if index === 0 || (index > 0 && sortedBuildRequests[index - 1].buildSystemName !== buildRequest.buildSystemName)}
					<Label text={buildRequest.buildSystemName} textSize="text-md" />
				{/if}
				<button
					class="w-full cursor-pointer"
					type="button"
					onclick={() => handleCollapsableQuestion(buildRequest.id)}
				>
					<div class="flex justify-between">
						<Label text={`Question: ${buildRequest.question}`} textSize="text-sm" />
						<CollapseIconAnimation collapse={collapsedQuestions.includes(buildRequest.id)} />
					</div>
				</button>
			</div>

			{#if collapsedQuestions.includes(buildRequest.id)}
				<div
					transition:slide={{ duration: 200, easing: sineIn }}
					class="grid w-full grid-cols-1 gap-2 md:grid-cols-3"
				>
					{#each buildRequest.answers.filter((a) => a.status === BuildAnswerStatus.None) as answer}
						<ItemDescription
							{menu}
							item={mapToBuildRequestAnswerType(answer, buildRequest.id)}
							handleItemClick={() =>
								onBuildItemClick(mapToBuildRequestAnswerType(answer, buildRequest.id))}
							title={answer.name}
							loading={loadingBuildSave}
						/>
					{/each}
				</div>
			{/if}
		{/each}
	{/if}
	{#if selectedItem}
		{@const selectedItemId = selectedItem.name}
		{#key selectedItemId}
			<Form
				close={handleItemCancel}
				handleSave={handleItemUpdate}
				saveButtonText="Update"
				schema={buildRequestAnswer}
				data={selectedItem}
			>
				<InputField name="Name" bind:value={selectedItem.name} />
				<InputField isTextArea name="Reason" bind:value={selectedItem.reason} />
				<InputField isTextArea name="Value" bind:value={selectedItem.template} />
			</Form>
		{/key}
	{/if}
{/if}
