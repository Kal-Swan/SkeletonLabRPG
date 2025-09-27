<script lang="ts">
	import Label from '@components/label.svelte';
	import SelectField from '@components/select-field.svelte';
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';
	import { clientFetch } from '@helpers/client-fetch';
	import { Actions } from '@models/actions';
	import ItemDescription from '@components/item-description.svelte';
	import type { llmBuildResponseType } from '@models/rpgbuild/llm-schema';
	import { type buildSchemaType, buildSchema } from '@models/rpgbuild/build-schema';
	import type { rpgSystemSchemaType } from '@models/rpgbuild/rpg-system-schema';
	import { rpgBuildQuestionSchema } from '@models/rpgbuild/llm-schema';
	import Select from '@components/select.svelte';

	let { data } = $props<{ rpgSystems: rpgSystemSchemaType[] }>();
	let { rpgSystems }: { rpgSystems: rpgSystemSchemaType[] } = data;
	let builds = $state<buildSchemaType[] | null>(null);
	let loadingAnswer = $state(false);
	let loadingBuildSave = $state<string>('');

	let rpgBuildQuestion = $state({
		question: '',
		rpgSystemId: ''
	});

	let template = $state({
		json: {}
	});

	let selectedItem = $state<buildSchemaType | null>(null);

	const menu = [
		{
			text: 'Edit',
			onClick: (text: string, item: buildSchemaType) => {
				selectedItem = item;
			}
		},
		{
			text: 'Delete',
			onClick: (text: string, item: buildSchemaType) => {
				builds = builds!.filter((build) => build.id !== item.id) || [];
			}
		},
		{
			text: 'Save',
			onClick: async (text: string, item: buildSchemaType) => {
				loadingBuildSave = item.id;
				const result = await clientFetch<buildSchemaType>('/llm', Actions.saveRpgBuild, item);

				if (result.isSuccess) {
					loadingBuildSave = '';
					builds = builds!.filter((build) => build.id !== item.id) || [];
				}
			}
		}
	];

	const onBuildItemClick = (item: buildSchemaType) => {
		selectedItem = item;
	};

	const handleItemUpdate = (item: buildSchemaType) => {
		if (!builds) {
			return;
		}
		const index = builds.findIndex((build) => build.id === item.id);
		builds[index] = item;
		selectedItem = null;
	};

	const handleItemCancel = () => {
		selectedItem = null;
	};

	async function handleCreateRpgBuildSave() {
		loadingAnswer = true;
		const result = await clientFetch<llmBuildResponseType>('/llm', Actions.createRpgBuilds, {
			question: rpgBuildQuestion.question,
			rpgSystem: rpgSystems.find((system) => system.id === rpgBuildQuestion.rpgSystemId)!.name
		});

		if (result.isSuccess) {
			const resultBuild = result.data as llmBuildResponseType;
			const newResult = resultBuild.builds.map((build) => ({
				...build,
				id: crypto.randomUUID(),
				rpgSystemId: rpgBuildQuestion.rpgSystemId
			}));
			builds = newResult;
		}
		loadingAnswer = false;
	}
</script>

<div class="mb-4">
	<Label text="Create Rpg Builds" />
</div>
<div>
	<Form
		data={rpgBuildQuestion}
		schema={rpgBuildQuestionSchema}
		handleSave={handleCreateRpgBuildSave}
		saveButtonText="Send"
		loading={loadingAnswer}
	>
		<Select options={rpgSystems} bind:value={rpgBuildQuestion.rpgSystemId} />

		<InputField
			name={'Question'}
			underline={false}
			type="text"
			placeholder="Enter question"
			bind:value={rpgBuildQuestion.question}
		/>
	</Form>
</div>

{#if builds}
	<div class="mb-4">
		<Label text="Suggested Rpg Builds" />
	</div>
	<div class="grid w-full grid-cols-1 gap-2 md:grid-cols-3">
		{#if !selectedItem}
			{#each builds as build}
				<ItemDescription
					{menu}
					item={build}
					handleItemClick={() => onBuildItemClick(build)}
					title={build.name}
					loading={loadingBuildSave}
				/>
			{/each}
		{/if}
	</div>
	{#if selectedItem}
		{@const selectedItemId = selectedItem.name}
		{#key selectedItemId}
			<Form
				close={handleItemCancel}
				handleSave={handleItemUpdate}
				saveButtonText="Update"
				schema={buildSchema}
				data={selectedItem}
			>
				<InputField name="Name" bind:value={selectedItem.name} />
				<InputField isTextArea name="Reason" bind:value={selectedItem.reason} />
				<InputField isTextArea name="Value" bind:value={selectedItem.template} />
			</Form>
		{/key}
	{/if}
{/if}
