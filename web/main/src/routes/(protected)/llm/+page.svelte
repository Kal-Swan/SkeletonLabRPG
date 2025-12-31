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
		type buildRequestType,
		type buildSystemType
	} from './build-details-schema.js';

	let { data } = $props();
	let {
		buildSystems,
		buildRequests
	}: { buildSystems: buildSystemType[]; buildRequests: buildRequestType[] } = data;
	// let builds = $state<buildSchemaType[]>(buildRequests.filter(request => request.status === BuildRequestStatus.Completed).flatMap(request => request.answers.map(answer => ({ ...answer, id: crypto.randomUUID(), buildSystemId: request.buildSystemId, question: request.question, buildRequestId: request.id, latestProcessedDate: request.latestProcessedDate }))) || null);
	let loadingAnswer = $state(false);
	let loadingBuildSave = $state<string>('');
	let currentBuildRequests = $state<buildRequestType[]>(
		buildRequests.filter((request) => request.status === BuildRequestStatus.Completed) || []
	);
	let currentProcessingRequests = $state<buildRequestType[]>(
		buildRequests.filter((request) => request.status === BuildRequestStatus.Processing) || []
	);
	let rpgBuildQuestion = $state({
		question: '',
		buildSystemId: ''
	});

	let selectedItem = $state<buildRequestAnswerType | null>(null);

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

			hubConnection.on('BuildCompleted', (data) => {
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

{#if currentProcessingRequests.some((request) => request.status === BuildRequestStatus.Processing)}
	<div class="mb-4">
		<Label text="Processing Requests" textSize="text-lg" />
		{#each currentProcessingRequests
			.filter((request) => request.status === BuildRequestStatus.Processing)
			.sort((a, b) => a.buildSystemName.localeCompare(b.buildSystemName)) as request}
			<Label text={request.buildSystemName} textSize="text-sm" />
			<p class="text-sm text-gray-700">Question: {request.question}</p>
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
				<Label text={`Question: ${buildRequest.question}`} textSize="text-sm" />
			</div>

			<div class="grid w-full grid-cols-1 gap-2 md:grid-cols-3">
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
