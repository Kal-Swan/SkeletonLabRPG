<script lang="ts">
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';
	import { createBuildSystemSchema } from '@models/rpgbuild/rpg-system-schema';
	import AddNewFile from './add-new-file.svelte';

	let {
		loading,
		newRpgSchema,
		handleSave,
		addNewFile,
		openNewFile,
		removeFile
	}: {
		loading: boolean;
		newRpgSchema: {
			name: string;
			files: File[];
		};
		handleSave: (data: { name: string; files: File[] }) => void;
		addNewFile: (event: Event) => void;
		openNewFile: (file: File) => void;
		removeFile: (fileName: string) => void;
	} = $props();
</script>

<Form {loading} schema={createBuildSystemSchema} data={newRpgSchema} {handleSave}>
	<div class="flex gap-2">
		<div class="flex flex-1 flex-col gap-2">
			<InputField name="Build System Name" bind:value={newRpgSchema.name} />
		</div>
		<div class="flex flex-col gap-2">
			<InputField
				name="Build System File"
				inputFieldId="buildSystemFile"
				type="file"
				onChange={addNewFile}
			/>
		</div>
	</div>
	<AddNewFile rpgSystem={newRpgSchema} {openNewFile} {removeFile} />
</Form>
