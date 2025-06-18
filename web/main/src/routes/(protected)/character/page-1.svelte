<script lang="ts">
	import CharacterAttributes from '@components/character-attributes.svelte';
	import { SuccessType } from '@models/character/success-type.js';
	import ItemSidebar from '@components/item-sidebar.svelte';
	import { Actions } from '@models/actions.js';
	import type { CharacterClassSchemaType } from '@models/character/classes/character-class-schema.js';
	import { ClassType, ClassTypeNameMap, classTypeOptions } from '@models/class-type.js';
	import { clientFetch, type Result } from '@helpers/client-fetch.js';
	import SelectField from '@components/select-field.svelte';
	import InputField from '@components/input-field.svelte';
	import type { Character } from '@models/character/character-simple.js';

	let { data } = $props();
	let { characters, configuration } = data;
	//let characterShare = configuration?.characterShare ?? 0;
	let classItems: { id: string; name: string; classType: ClassType }[] = $state([]);
	let className: string = $state('');
	let createdClassType: ClassType | undefined = $state();
	let newCharacterName: string = $state('');
	let selectedCharacterClassId = $state('');
	let selectedCharacterId = $state();
	let selectedClassType: ClassType | undefined = $state();

	async function handleSelectedCharacter(id: string) {
		selectedCharacterId = id;
		const response = await clientFetch<{ id: string }>('/', Actions.getCharacterClasses, {
			id
		});

		const classes = response.data as CharacterClassSchemaType[];

		classItems = classes.map((item) => ({
			id: item.id,
			name: item.name,
			classType: item.classType,
			subtitle: ClassTypeNameMap[item.classType]
		}));
	}

	function successResponse(successType: SuccessType) {
		if (successType === SuccessType.updatedName) console.log('Update success');
		if (successType === SuccessType.deletedCharacter) console.log('Delete success');
	}

	function classSuccessResponse(successType: SuccessType) {}

	async function handleNewClass() {
		await clientFetch('/', Actions.createCharacterClass, {
			id: selectedCharacterId,
			name: className,
			classType: createdClassType
		});
	}

	function handleSelectedClass(id: string) {
		selectedCharacterClassId = id;
		selectedClassType = classItems.find((item) => item.id === id)?.classType;
	}

	async function handleNewCharacter() {
		const result = await clientFetch('/character', Actions.createCharacter, {
			newCharacterName
		});
		console.log(result);

		// if (!result.isSuccess) {
		// 	errorMessage.set(['Failed to create new character']);
		// 	logError(result.errors);
		// 	return;
		// }

		var newCharacter = result.data as Character;
		characters.push(newCharacter);
	}
</script>

<div class="flex h-screen flex-col">
	<div class="flex flex-grow">
		<aside class="sidebar">
			<ItemSidebar
				handleNewItem={handleNewCharacter}
				{successResponse}
				items={characters}
				handleSelectedItem={handleSelectedCharacter}
			>
				<InputField type="text" bind:value={newCharacterName} />
			</ItemSidebar>
		</aside>
		<aside class="sidebar">
			<ItemSidebar
				handleNewItem={handleNewClass}
				successResponse={classSuccessResponse}
				items={classItems}
				handleSelectedItem={handleSelectedClass}
			>
				<InputField type="text" bind:value={className} />
				<SelectField bind:value={createdClassType} options={classTypeOptions} />
			</ItemSidebar>
		</aside>
		{#if selectedCharacterClassId.length > 0}
			<aside class="detail-panel">
				<CharacterAttributes id={selectedCharacterClassId} classType={selectedClassType!} />
			</aside>
			<aside class="result-panel"></aside>
		{/if}
	</div>
</div>

<style>
	.sidebar {
		margin-left: 10px;
		width: 20%;
		background-color: rgba(107, 114, 128, 0.2); /* Gray-700 with 70% opacity */
		padding: 1rem 0;
		color: white;
		transition: all 0.3s ease-in-out;
	}

	.detail-panel {
		transition: all 0.3s ease-in-out;
		background-color: rgba(75, 85, 99, 0.7); /* Gray-600 with 70% opacity */
		padding: 1rem;
		color: white;
		margin-left: 10px;
		margin-right: 10px;
		width: 50%;
	}

	.result-panel {
		transition: all 0.3s ease-in-out;
		background-color: rgba(75, 85, 99, 0.7); /* Gray-600 with 70% opacity */
		padding: 1rem;
		color: white;
		margin-right: 10px;
	}

	/* .shine-button {
		position: relative;
		overflow: hidden;
	}

	.shine-button::before {
		content: '';
		position: absolute;
		top: -50%;
		left: -50%;
		width: 200%;
		height: 200%;
		background: rgba(255, 255, 255, 0.5);
		transform: rotate(45deg);
		transition: all 0.5s ease;
	}

	.shine-button:hover::before {
		top: -50%;
		left: 100%;
	} */
</style>
