<script lang="ts">
	import {
		type buildSystemSchemaType,
		type createBuildSystemSchemaType
	} from '@models/rpgbuild/rpg-system-schema';
	import Label from '@components/label.svelte';
	import { clientFetch } from '@helpers/client-fetch';
	import { Actions } from '@models/actions';
	import { notificationMessages } from '@lib/stores/notification-message';
	import { NotificationType } from '@models/notification.types';
	import { slide } from 'svelte/transition';
	import ExistingSystemFilesForm from './existing-system-files-form.svelte';
	import NewSystemFilesForm from './new-system-files-form.svelte';
	import CollapseIconAnimation from '@components/collapse-icon-animation.svelte';

	let { data } = $props<{ rpgSystems: buildSystemSchemaType[] }>();
	let { rpgSystems }: { rpgSystems: buildSystemSchemaType[] } = data;
	let newRpgSystemLoading = $state<boolean>(false);
	let existingRpgSystemLoading = $state<boolean>(false);
	let openingFile = $state<boolean>(false);
	let fileCount = $state<number>(0);
	let savedRpgSystems = $state<buildSystemSchemaType[]>(rpgSystems ?? []);
	let showCurrentSystemFilesForm = $state<boolean>(true);
	let showNewSystemFilesForm = $state<boolean>(true);
	let innerWidth = $state<number>(0);

	let isMediumScreen = $derived(innerWidth > 600);

	$effect(() => {
		if (isMediumScreen) {
			showCurrentSystemFilesForm = true;
			showNewSystemFilesForm = true;
		}
	});

	let newRpgSchema = $state<createBuildSystemSchemaType>({
		name: '',
		files: []
	});

	let currentRpgSystemsOptions = $state<{ id: string; name: string }[]>(
		rpgSystems?.map((system) => ({ id: system.id, name: system.name })) ?? []
	);

	let currentRpgSystem = $state<buildSystemSchemaType>({
		id: '',
		name: '',
		fileNames: [],
		files: []
	});

	const handleSave = async () => {
		newRpgSystemLoading = true;
		console.log('Saving new RPG System:', newRpgSchema);
		const saved = await clientFetch<buildSystemSchemaType>(
			'/rpgsystem',
			Actions.createRpgSystem,
			newRpgSchema
		);
		if (saved.isSuccess) {
			currentRpgSystemsOptions = [...currentRpgSystemsOptions, saved.data!];
			newRpgSchema = { name: '', files: [] };
			savedRpgSystems = [...savedRpgSystems, saved.data!];
			fileCount = 0;
			notificationMessages.set([
				{
					id: crypto.randomUUID(),
					type: NotificationType.Success,
					message: 'Saved! Please find new system in dropdown.'
				}
			]);
		}
		newRpgSystemLoading = false;
	};
	const handleCurrentDelete = async () => {
		if (currentRpgSystem.id === '') {
			notificationMessages.set([
				{
					id: crypto.randomUUID(),
					type: NotificationType.Error,
					message: 'Please select a system to delete.'
				}
			]);
			return;
		}
		existingRpgSystemLoading = true;
		const deleted = await clientFetch('/rpgsystem', Actions.deleteRpgSystem, currentRpgSystem);
		if (deleted.isSuccess) {
			existingRpgSystemLoading = false;
			currentRpgSystemsOptions = currentRpgSystemsOptions.filter(
				(system) => system.id !== currentRpgSystem.id
			);
			currentRpgSystem = { id: '', name: '', fileNames: [], files: [] };
		}
	};
	const handleCurrentUpdate = async () => {
		existingRpgSystemLoading = true;
		const updated = await clientFetch<buildSystemSchemaType>(
			'/rpgsystem',
			Actions.updateRpgSystem,
			currentRpgSystem
		);
		if (updated.isSuccess) {
			existingRpgSystemLoading = false;
			currentRpgSystem = updated.data!;
			currentRpgSystemsOptions = currentRpgSystemsOptions.map((system) =>
				system.id === updated.data!.id ? updated.data! : system
			);
		}
	};

	const openFile = async (id: string, fileName: string) => {
		openingFile = true;
		const result = await clientFetch(
			'/rpgsystem',
			Actions.openBuildSystemFile,
			{
				id,
				fileName
			},
			true
		);
		const blob = await (result.response as Response).blob()!;
		const url = URL.createObjectURL(blob);
		openingFile = false;
		window.open(url, '_blank');
	};

	const openNewFile = (file: File) => {
		const url = URL.createObjectURL(file);
		window.open(url, '_blank');
	};

	const addNewFile = (event: Event) => {
		const input = event.target as HTMLInputElement;
		const file = input.files![0];
		console.log('Adding file:', file);
		newRpgSchema.files.push(file);
		input.value = '';
	};

	const addNewFileToExistingBuildSystem = (event: Event) => {
		const input = event.target as HTMLInputElement;
		const file = input.files![0];
		console.log('Adding file to existing system:', file);
		currentRpgSystem.files = currentRpgSystem.files ?? [];
		currentRpgSystem.files.push(file);
		input.value = '';
	};

	function onCurrentBuildSystemChange(id: string) {
		console.log('selected id');
		console.log(id);

		currentRpgSystem = savedRpgSystems.find((system) => system.id === id)!;
	}

	function removeFileNameFromCurrentBuildSystem(fileName: string) {
		currentRpgSystem = {
			...currentRpgSystem,
			fileNames: currentRpgSystem.fileNames?.filter((name) => name !== fileName)
		};
	}

	function removeFileFromCurrentBuildSystem(fileName: string) {
		currentRpgSystem.files = currentRpgSystem.files?.filter((file) => file.name !== fileName);
	}

	function removeFile(fileName: string) {
		newRpgSchema.files = newRpgSchema.files?.filter((file) => file.name !== fileName);
	}

	function handleShowCurrentSystemForm() {
		showCurrentSystemFilesForm = !showCurrentSystemFilesForm;
	}
	function handleShowNewSystemFilesForm() {
		showNewSystemFilesForm = !showNewSystemFilesForm;
	}
</script>

<svelte:window bind:innerWidth />
<div class="flex w-full flex-col justify-evenly gap-2 lg:flex-row">
	{#if currentRpgSystemsOptions?.length > 0}
		<div class="w-full rounded-md border-1 border-white p-5">
			<div class="sm:block md:hidden">
				<button class="w-full cursor-pointer" type="button" onclick={handleShowCurrentSystemForm}>
					<div class="flex justify-between">
						<Label text="Edit Saved Build Systems" />
						<CollapseIconAnimation collapse={showCurrentSystemFilesForm} />
					</div>
				</button>
			</div>
			<div class="hidden sm:hidden md:block">
				<Label text="Edit Saved Build Systems" />
			</div>
			{#if showCurrentSystemFilesForm}
				<div transition:slide={{ duration: 400 }} class="mt-5 flex flex-col gap-3">
					<ExistingSystemFilesForm
						rpgSystemOptions={currentRpgSystemsOptions}
						onBuildSystemChange={onCurrentBuildSystemChange}
						rpgSystem={currentRpgSystem}
						handleDelete={handleCurrentDelete}
						handleUpdate={handleCurrentUpdate}
						loading={existingRpgSystemLoading}
						{openingFile}
						removeFileName={removeFileNameFromCurrentBuildSystem}
						removeFile={removeFileFromCurrentBuildSystem}
						addNewFile={addNewFileToExistingBuildSystem}
						{openNewFile}
						{openFile}
					/>
				</div>
			{/if}
		</div>
	{/if}
	<div class="w-full rounded-md border-1 border-white p-5">
		<div class="md:hidden">
			<button class="w-full cursor-pointer" type="button" onclick={handleShowNewSystemFilesForm}>
				<div class="flex justify-between">
					<Label text="Create New RPG System" />
					<CollapseIconAnimation collapse={showNewSystemFilesForm} />
				</div>
			</button>
		</div>
		<div class="hidden sm:hidden md:block">
			<Label text="Create New RPG System" />
		</div>
		{#if showNewSystemFilesForm}
			<div transition:slide={{ duration: 400 }} class="mt-5 flex flex-col gap-3">
				<NewSystemFilesForm
					{addNewFile}
					{handleSave}
					{openNewFile}
					{removeFile}
					loading={existingRpgSystemLoading}
					{newRpgSchema}
				/>
			</div>
		{/if}
	</div>
</div>
