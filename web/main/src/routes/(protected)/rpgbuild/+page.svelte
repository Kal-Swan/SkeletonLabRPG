<script lang="ts">
	import Label from '@components/label.svelte';
	import ItemDescription from '@components/item-description.svelte';
	import { clientFetch } from '@helpers/client-fetch';
	import { Actions } from '@models/actions';
	import { buildSchema, type buildSchemaType } from '@models/rpgbuild/build-schema.js';
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';

	type groupedSavedBuildsType = Record<string, buildSchemaType[]>;
	let { data } = $props();
	let { builds } = data;
	let savedBuilds = $state<groupedSavedBuildsType | null>(builds);
	let selectedItem = $state<buildSchemaType | null>(null);
	let loadingBuildId = $state<string>('');
	let selectedKey = $state<string>('');

	const onBuildItemClick = (item: buildSchemaType, key: string) => {
		selectedItem = item;
		selectedKey = key;
	};

	const menu = [
		{
			text: 'Edit',
			onClick: (text: string, item: buildSchemaType) => {
				selectedItem = item;
				console.log('edit clicked: ' + text);
			}
		},
		{
			text: 'Delete',
			onClick: async (text: string, item: buildSchemaType) => {
				console.log('delete clicked: ' + text);
				loadingBuildId = item.id;
				console.log(item);
				const result = await clientFetch<buildSchemaType>('/rpgbuild', Actions.deleteRpgBuild, {
					id: item.id
				});

				if (result.isSuccess) {
					loadingBuildId = '';
					console.log('deleted build');
					console.log(item.rpgSystemId);
					savedBuilds = {
						...savedBuilds,
						[selectedKey]: savedBuilds![selectedKey].filter(
							(build) => build.id !== item.id
						)
					};
				}
			}
		}
	];

	const handleItemUpdate = async (item: buildSchemaType) => {
		loadingBuildId = item.id;
		const result = await clientFetch<buildSchemaType>('/rpgbuild', Actions.updateRpgBuild, item);

		loadingBuildId = '';
		if (result.isSuccess) {
			savedBuilds = {
				...savedBuilds,
				[selectedKey]: savedBuilds![selectedKey].map((build) =>
					build.id === result.data!.id ? { ...build, ...result.data } : build
				)
			};
		}
	};

	const handleItemCancel = () => {
		selectedItem = null;
	};

	$effect(() => {
		console.log('savedBuilds changed: ');
		console.log(savedBuilds);
	});

</script>

{#if savedBuilds && !selectedItem}
	{#each Object.keys(savedBuilds) as key}
		<div class="mt-5 flex flex-col">
			<Label text={key} textSize="text-md" />
			<div class="mt-5 grid w-full grid-cols-1 gap-2 md:grid-cols-3">
				{#each savedBuilds[key] as build}
					<ItemDescription
						{menu}
						item={build}
						handleItemClick={() => onBuildItemClick(build, key)}
						title={build.name}
						loading={loadingBuildId}
					/>
				{/each}
			</div>
		</div>
	{/each}
{/if}
{#if selectedItem}
	{@const selectedItemId = selectedItem.id}
	{#key selectedItemId}
		<Form
			close={handleItemCancel}
			handleSave={handleItemUpdate}
			saveButtonText="Update"
			schema={buildSchema}
			data={selectedItem}
			loading={loadingBuildId === selectedItemId}
		>
			<InputField name="Name" bind:value={selectedItem.name} />
			<InputField name="Reason" bind:value={selectedItem.reason} />
			<InputField isTextArea name="Value" bind:value={selectedItem.template} />
		</Form>
	{/key}
{/if}
