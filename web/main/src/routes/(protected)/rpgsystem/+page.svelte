<script lang="ts">
	import Form from '@components/form.svelte';
	import InputField from '@components/input-field.svelte';
	import {
		rpgSystemSchema,
		type rpgSystemSchemaType,
		type createRpgSystemSchemaType,
		createRpgSystemSchema
	} from '@models/rpgbuild/rpg-system-schema';
	import Label from '@components/label.svelte';
	import { clientFetch } from '@helpers/client-fetch';
	import { Actions } from '@models/actions';
	import { notificationMessages } from '@lib/stores/notification-message';
	import { NotificationType } from '@models/notification.types';
	import Select from '@components/select.svelte';

	let { data } = $props<{ rpgSystems: rpgSystemSchemaType[] }>();
	let { rpgSystems } = data;
	let newRpgSystemLoading = $state<boolean>(false);
	let existingRpgSystemLoading = $state<boolean>(false);

	let newRpgSchema = $state<createRpgSystemSchemaType>({
		name: ''
	});

	let currentRpgSystems = $state<rpgSystemSchemaType[]>(rpgSystems ?? []);

	let currentRpgSystem = $state<rpgSystemSchemaType>({
		id: '',
		name: ''
	});

	const handleSave = async () => {
		newRpgSystemLoading = true;
		const saved = await clientFetch<rpgSystemSchemaType>(
			'/rpgsystem',
			Actions.createRpgSystem,
			newRpgSchema
		);
		if (saved.isSuccess) {
			currentRpgSystems = [...currentRpgSystems, saved.data!];
			newRpgSchema = { name: '' };
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
			currentRpgSystems = currentRpgSystems.filter(
				(system: rpgSystemSchemaType) => system.id !== currentRpgSystem.id
			);
			currentRpgSystem = { id: '', name: '' };
		}
	};
	const handleCurrentUpdate = async () => {
		existingRpgSystemLoading = true;
		const updated = await clientFetch('/rpgsystem', Actions.updateRpgSystem, currentRpgSystem);
		if (updated.isSuccess) {
			existingRpgSystemLoading = false;
			currentRpgSystems = currentRpgSystems.map((system: rpgSystemSchemaType) =>
				system.id === currentRpgSystem.id ? currentRpgSystem : system
			);
		}
	};

	$effect(() => {
		if (currentRpgSystem.id) {
			currentRpgSystem.name = currentRpgSystems.find(
				(system) => system.id === currentRpgSystem.id
			)!.name;
		}
	});
</script>

<div class="flex w-full flex-col justify-evenly gap-2 lg:flex-row">
	{#if currentRpgSystems?.length > 0}
		<div class="w-full rounded-md border-1 border-white p-5">
			<Label text="Edit Saved RPG Systems" />
			<div class="mt-5">
				<Form
					schema={rpgSystemSchema}
					data={currentRpgSystem}
					saveButtonText="Update"
					handleDelete={handleCurrentDelete}
					handleSave={handleCurrentUpdate}
					loading={existingRpgSystemLoading}
				>
					<Select bind:value={currentRpgSystem.id} options={currentRpgSystems} />
					<InputField name="RPG System Name" bind:value={currentRpgSystem.name} />
				</Form>
			</div>
		</div>
	{/if}
	<div class="w-full rounded-md border-1 border-white p-5">
		<Label text="Create New RPG System" isHeader />

		<Form
			loading={newRpgSystemLoading}
			schema={createRpgSystemSchema}
			data={newRpgSchema}
			{handleSave}
		>
			<InputField name="RPG System Name" bind:value={newRpgSchema.name} />
		</Form>
	</div>
</div>
