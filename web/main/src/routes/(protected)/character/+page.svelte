<script lang="ts">
	import Confirmation from '@components/confirmation.svelte';
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';
	import ItemDescription from '@components/item-description.svelte';
	import Label from '@components/label.svelte';
	import Loading from '@components/loading.svelte';
	import TextButton from '@components/text-button.svelte';
	import { clientFetch } from '@helpers/client-fetch.js';
	import { Actions } from '@models/actions';
	import {
		characterSchema,
		createCharacterSchema,
		updateCharacterSchema,
		type characterSchemaType
	} from '@models/character/character-schema';

	let { data } = $props();
	let { characters } = data;
	let currentCharacters = $state(characters ?? []);
	let openConfirmation = $state('');
	let selectedCharacterId = $state<string | null>(null);

	let newCharacter = $state({
		name: ''
	});

	let selectedCharacterName = $state('');
	let loading = $state(false);

	async function handleCreateCharacterSave() {
		console.log('Creating character with name: ' + newCharacter.name);
		const result = await clientFetch<characterSchemaType>('/character', Actions.createCharacter, {
			name: newCharacter.name
		});

		if (result.isSuccess) {
			currentCharacters = [...currentCharacters, result.data!];
			newCharacter.name = '';
		}
	}

	async function handleUpdateCharacterNameSave() {
		const result = await clientFetch<characterSchemaType>(
			'/character',
			Actions.updateCharacterName,
			{
				name: selectedCharacterName,
				id: selectedCharacterId
			}
		);

		if (result.isSuccess) {
			console.log('Character name updated: ');
			console.log(result.data!.name);
			openConfirmation = '';
			currentCharacters = currentCharacters.map((character) =>
				character.id === selectedCharacterId ? { ...character, name: result.data!.name } : character
			);
		}
	}

	function onCharacterItemClick(id: string) {
		selectedCharacterId = id;
	}

	const menu = [
		{
			text: 'Edit Name',
			onClick: (text: string) => {
				openConfirmation = text;
				console.log('edit clicked: ' + text);
			}
		},
		{
			text: 'Delete',
			onClick: (text: string) => {
				openConfirmation = text;
				console.log('delete clicked: ' + text);
			}
		}
	];

	const deleteConfirmation = {
		title: 'Delete Character',
		message:
			'Deleting this character deletes all builds belonging to this character. Are you sure you want to delete this character?',
		button: {
			text: 'Delete',
			onClick: async () => {
				console.log('Character deleted' + selectedCharacterId);
				loading = true;
				try {
					const result = await clientFetch('/character', Actions.deleteCharacter, {
						id: selectedCharacterId
					});

					if (result.isSuccess) {
						currentCharacters = currentCharacters.filter(
							(character) => character.id !== selectedCharacterId
						);
					}
				} finally {
					loading = false;
					openConfirmation = '';
				}
			},
			actionType: Actions.deleteCharacter
		}
	};

	const editNameConfirmation = {
		title: 'Edit Character Name',
		message: 'Please enter a new name for the character.',
		button: {
			text: 'Save',
			onClick: () => {
				console.log('Character name edited' + selectedCharacterId);
				openConfirmation = '';
			},
			actionType: Actions.updateCharacterName
		}
	};

	function closeConfirmation() {
		openConfirmation = '';
	}
</script>

<div class="mb-4">
	<Label text="Create Character" />
</div>
<div>
	<Form data={newCharacter} schema={createCharacterSchema} handleSave={handleCreateCharacterSave}>
		<InputField
			name={'name'}
			underline={false}
			type="text"
			placeholder="Enter character name"
			bind:value={newCharacter.name}
		/>
	</Form>
</div>
<div class="mt-5 flex flex-wrap gap-5">
	{#each currentCharacters as character}
		<ItemDescription
			{menu}
			handleItemClick={onCharacterItemClick}
			id={character.id}
			title={character.name}
		>
			<div class="text-xs uppercase">builds: 0</div>
		</ItemDescription>
	{/each}
	{#if openConfirmation === 'Delete' && selectedCharacterId}
		<Confirmation
			id={selectedCharacterId}
			message={deleteConfirmation.message}
			bind:open={openConfirmation}
			title={deleteConfirmation.title}
			{loading}
			button={{
				actionType: deleteConfirmation.button.actionType,
				text: deleteConfirmation.button.text,
				onClick: () => deleteConfirmation.button.onClick()
			}}
		/>
	{/if}
	{#if openConfirmation === 'Edit Name' && selectedCharacterId}
		<Confirmation
			id={selectedCharacterId}
			message={editNameConfirmation.message}
			bind:open={openConfirmation}
			title={editNameConfirmation.title}
		>
			<Form
				data={newCharacter}
				schema={updateCharacterSchema}
				handleSave={handleUpdateCharacterNameSave}
				close={closeConfirmation}
			>
				<InputField
					name={'name'}
					underline={false}
					type="text"
					placeholder="Enter new character name"
					bind:value={selectedCharacterName}
				/>
			</Form>
		</Confirmation>
	{/if}
</div>
