<script lang="ts">
	import Form from '@components/form.svelte';
	import IconButton from '@components/icon-button.svelte';
	import InputField from '@components/input-field.svelte';
	import Label from '@components/label.svelte';
	import Select from '@components/select.svelte';
	import TextButton from '@components/text-button.svelte';
	import { ButtonType, IconType } from '@models/Icon-type';
	import {
		buildSystemSchema,
		type buildSystemSchemaType
	} from '@models/rpgbuild/rpg-system-schema';
	import AddNewFile from './add-new-file.svelte';

	type RpgSystemOption = {
		id: string;
		name: string;
	};

	let {
		rpgSystemOptions,
		onBuildSystemChange,
		rpgSystem,
		handleDelete,
		handleUpdate,
		openFile,
		loading,
		openingFile,
		addNewFile,
		removeFile,
		openNewFile,
		removeFileName
	}: {
		rpgSystemOptions: RpgSystemOption[];
		onBuildSystemChange(id: string): void;
		rpgSystem: buildSystemSchemaType;
		handleDelete: () => Promise<void>;
		handleUpdate: () => Promise<void>;
		openFile: (id: string, fileName: string) => Promise<void>;
		loading: boolean;
		openingFile: boolean;
		addNewFile: (event: Event) => void;
		openNewFile: (file: File) => void;
		removeFileName(fileName: string): void;
		removeFile(fileName: string): void;
	} = $props();
</script>

<Select onChange={onBuildSystemChange} options={rpgSystemOptions} />
<Form
	schema={buildSystemSchema}
	data={rpgSystem}
	saveButtonText="Update"
	{handleDelete}
	handleSave={handleUpdate}
	{loading}
>
	<InputField name="Build System Name" bind:value={rpgSystem.name} />
	{#each rpgSystem.fileNames as fileName, index (index)}
		<div class="mb-2 flex items-center gap-2">
			<Label text={fileName} textSize="sm" />
			<TextButton
				loading={openingFile}
				text="Open File"
				onClick={() => openFile(rpgSystem.id, fileName)}
			/>
			<TextButton text="Remove File" onClick={() => removeFileName(fileName)} />
		</div>
	{/each}
	<InputField
		name="Add new file"
		inputFieldId="addNewFileToExistingBuildSystem"
		type="file"
		onChange={addNewFile}
	/>
	<AddNewFile {rpgSystem} {openNewFile} {removeFile} />
</Form>
