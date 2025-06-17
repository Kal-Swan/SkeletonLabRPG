<script lang="ts">
	import { ButtonType, IconType } from '@models/Icon-type';
	import Button from './icon-button.svelte';

	import InputField from './input-field.svelte';
	import { error } from '@sveltejs/kit';
	import { SuccessType } from '@models/character/success-type';
	import ClickableItem from './clickable-item.svelte';

	let { id, name, subtitle, success, selected } = $props<{
		id?: string;
		name: string;
		subtitle?: string;
		success?: (type: SuccessType) => void;
		selected?: (id: string) => void;
		handleAction: () => void;
	}>();
	let edit = $state(false);
	let showCharacterDetails = $state(false);

	async function handleUpdate() {
		try {
			let result = await fetch('/', {
				method: 'POST',
				headers: {
					'Content-Type': 'application/json'
				},
				body: JSON.stringify({ id, name, action: 'update' })
			});

			let data = await result.json();

			if (data.status === 200) {
				return success(SuccessType.updatedName);
			}

			return error(400, 'Failed to update');
		} catch (e) {
			console.log(e);
		}
	}

	async function handleDelete() {
		let result = await fetch('/', {
			method: 'POST',
			headers: {
				'Content-Type': 'application/json'
			},
			body: JSON.stringify({ id, name, action: 'delete' })
		});

		if (result.ok) {
			return success(SuccessType.deletedCharacter);
		}

		return error(400, 'Failed to delete');
	}

	function handleEdit() {
		edit = !edit;
	}

	function handleClick() {
		showCharacterDetails = !showCharacterDetails;
		selected(id);
	}

	function handleCancel() {
		edit = false;
	}
</script>

<div class="flex flex-row items-center">
	<div class="editable-item">
		{#if edit}
			<div class="flex w-full flex-col">
				<div>
					<InputField type="text" bind:value={name} placeholder="Update name" />
				</div>
				<div class="flex w-full justify-between">
					<div class="flex pl-2">
						<Button
							buttonType={ButtonType.cancel}
							iconType={IconType.cancel}
							onClick={handleCancel}
						/>
					</div>
					<div class="flex pr-2">
						<Button buttonType={ButtonType.add} iconType={IconType.add} onClick={handleUpdate} />
						<Button
							buttonType={ButtonType.delete}
							iconType={IconType.minus}
							onClick={handleDelete}
						/>
					</div>
				</div>
			</div>
		{:else}
			<div class="flex w-full flex-col pl-2">
				<div>
					<ClickableItem {subtitle} text={name} clicked={showCharacterDetails} {handleClick} />
				</div>
				<div class="flex justify-end pr-2">
					<Button buttonType={ButtonType.edit} iconType={IconType.edit} onClick={handleEdit} />
				</div>
			</div>
		{/if}
	</div>
</div>

<style>
	.editable-item {
		display: flex;
		align-items: center;
		justify-content: space-between;
		margin-bottom: 1rem;
		width: 100%;
	}
</style>
