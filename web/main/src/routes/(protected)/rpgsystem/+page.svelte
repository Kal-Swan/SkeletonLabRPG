<script lang="ts">
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';
	import {
		buildSystemSchema,
		type buildSystemSchemaType,
		type createBuildSystemSchemaType,
		createBuildSystemSchema
	} from '@models/rpgbuild/rpg-system-schema';
	import Label from '@components/label.svelte';
	import { clientFetch } from '@helpers/client-fetch';
	import { Actions } from '@models/actions';
	import { notificationMessages } from '@lib/stores/notification-message';
	import { NotificationType } from '@models/notification.types';
	import Select from '@components/select.svelte';
	import TextButton from '@components/text-button.svelte';
	import { rpgSystemEndpoint } from './endpoint';
	import IconButton from '@components/icon-button.svelte';
	import { ButtonType, IconType } from '@models/Icon-type';

	let { data } = $props<{ rpgSystems: buildSystemSchemaType[] }>();
	let { rpgSystems }: { rpgSystems: buildSystemSchemaType[] } = data;
	let newRpgSystemLoading = $state<boolean>(false);
	let existingRpgSystemLoading = $state<boolean>(false);
	let openingFile = $state<boolean>(false);
	let fileCount = $state<number>(0);
	let savedRpgSystems = $state<buildSystemSchemaType[]>(rpgSystems ?? []);

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
		// existingRpgSystemLoading = true;
		// const updated = await clientFetch('/rpgsystem', Actions.updateRpgSystem, currentRpgSystem);
		// if (updated.isSuccess) {
		// 	existingRpgSystemLoading = false;
		// 	currentRpgSystemsOptions = currentRpgSystemsOptions.map((system: buildSystemSchemaType) =>
		// 		system.id === currentRpgSystem.id ? currentRpgSystem : system
		// 	);
		// }
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

	// $effect(() => {
	// 	if (currentRpgSystem.id) {
	// 		currentRpgSystem = currentRpgSystems.find(
	// 			(system) => system.id === currentRpgSystem.id
	// 		);
	// 	}
	// });

	$effect(() => {
		console.log('options');
		console.log(currentRpgSystemsOptions);
	});

	function onCurrentBuildSystemChange(id: string) {
		console.log('selected id');
		console.log(id);

		currentRpgSystem = savedRpgSystems.find((system) => system.id === id)!;
	}

	function removeFileNameFromCurrentBuildSystem(fileName: string) {
		currentRpgSystem = {
			...currentRpgSystem,
			fileNames: currentRpgSystem.fileNames.filter((name) => name !== fileName)
		};
	}

	function removeFileFromCurrentBuildSystem(fileName: string) {
		currentRpgSystem.files = currentRpgSystem.files?.filter((file) => file.name !== fileName);
	}

	function removeFile(fileName: string) {
		newRpgSchema.files = newRpgSchema.files?.filter((file) => file.name !== fileName);
	}
</script>

<div class="flex w-full flex-col justify-evenly gap-2 lg:flex-row">
	{#if currentRpgSystemsOptions?.length > 0}
		<div class="w-full rounded-md border-1 border-white p-5">
			<Label text="Edit Saved Build Systems" />
			<div class="mt-5 flex flex-col gap-3">
				<Select onChange={onCurrentBuildSystemChange} options={currentRpgSystemsOptions} />

				<Form
					schema={buildSystemSchema}
					data={currentRpgSystem}
					saveButtonText="Update"
					handleDelete={handleCurrentDelete}
					handleSave={handleCurrentUpdate}
					loading={existingRpgSystemLoading}
				>
					<InputField name="Build System Name" bind:value={currentRpgSystem.name} />
					{#each currentRpgSystem.fileNames as fileName, index (index)}
						<div class="mb-2 flex items-center gap-2">
							<Label text={fileName} textSize="sm" />
							<TextButton
								loading={openingFile}
								text="Open File"
								onClick={() => openFile(currentRpgSystem.id, fileName)}
							/>
							<TextButton
								text="Remove File"
								onClick={() => removeFileNameFromCurrentBuildSystem(fileName)}
							/>
						</div>
					{/each}
					<InputField
						name="Add new file"
						inputFieldId="addNewFileToExistingBuildSystem"
						type="file"
						onChange={addNewFileToExistingBuildSystem}
					/>
					{#if currentRpgSystem.files != null && currentRpgSystem.files.length > 0}
						<Label text="Files added:" textSize="sm" />
					{/if}

					<div class="grid w-full grid-cols-1 gap-2 md:grid-cols-3">
						{#each currentRpgSystem.files as file, index (index)}
							<div
								class="mb-2 flex justify-between sm:w-full sm:justify-start md:items-center md:gap-2"
							>
								<Label text={file.name} textSize="sm" />
								<div>
									<IconButton
										buttonType={ButtonType.add}
										iconType={IconType.open}
										onClick={() => openNewFile(file)}
									/>
									<IconButton
										buttonType={ButtonType.delete}
										iconType={IconType.delete}
										onClick={() => removeFileFromCurrentBuildSystem(file.name)}
									/>
								</div>
							</div>
						{/each}
					</div>
				</Form>
			</div>
		</div>
	{/if}
	<div class="w-full rounded-md border-1 border-white p-5">
		<Label text="Create New RPG System" isHeader />

		<Form
			loading={newRpgSystemLoading}
			schema={createBuildSystemSchema}
			data={newRpgSchema}
			{handleSave}
		>
			<div class="flex gap-2">
				<div class="flex flex-1 flex-col gap-2">
					<InputField name="Build System Name" bind:value={newRpgSchema.name} />
				</div>
				<div class="flex flex-1 flex-col gap-2">
					<InputField
						name="Build System File"
						inputFieldId="buildSystemFile"
						type="file"
						onChange={addNewFile}
					/>
				</div>
			</div>

			{#if newRpgSchema.files.length > 0}
				<Label text="Files added:" textSize="sm" />
			{/if}

			<div class="grid w-full grid-cols-1 gap-2 md:grid-cols-3">
				{#each newRpgSchema.files as file, index (index)}
					<div
						class="mb-2 flex justify-between sm:w-full sm:justify-start md:items-center md:gap-2"
					>
						<Label text={file.name} textSize="sm" />
						<div>
							<IconButton
								buttonType={ButtonType.add}
								iconType={IconType.open}
								onClick={() => openNewFile(file)}
							/>
							<IconButton
								buttonType={ButtonType.delete}
								iconType={IconType.delete}
								onClick={() => removeFile(file.name)}
							/>
						</div>
					</div>
				{/each}
			</div>
		</Form>
	</div>
</div>
