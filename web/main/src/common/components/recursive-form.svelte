<script lang="ts">
	import ItemDescription from './v2/item-description.svelte';
	import RecursiveItems from './recursive-items.svelte';
	import InputField from './input-field.svelte';
	import Form from './form.svelte';
	import { buildSchema, type buildSchemaType } from '@models/rpgbuild/build-schema';

	let {
		build = $bindable(),
		handleItemUpdate,
		handleItemCancel,
		saveButtonText = 'Save'
	} = $props<{
		build: Record<string, any>;
		handleItemUpdate: (item: buildSchemaType) => void;
		handleItemCancel: (item: buildSchemaType) => void;
		saveButtonText?: string;
	}>();

	let currentBuild = $state<buildSchemaType>(build);

	const handleSave = () => {
		handleItemUpdate(build);
	};

	const handleCancel = () => {
		handleItemCancel(build);
	};
</script>

<div>
	<Form data={currentBuild} schema={buildSchema} {handleSave} {saveButtonText} close={handleCancel}>
		<RecursiveItems bind:build={currentBuild} />
	</Form>
</div>
