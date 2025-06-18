<script lang="ts">
	import EditableItem from './editable-item.svelte';
	import { ButtonType, IconType } from '@models/Icon-type';
	import Button from './icon-button.svelte';
	import { SuccessType } from '@models/character/success-type';
	import TextButton from './text-button.svelte';
	import type { Snippet } from 'svelte';

	let { items, handleSelectedItem, successResponse, handleNewItem, children } = $props<{
		items: any[];
		handleSelectedItem: (id: string) => void;
		successResponse: (successType: SuccessType) => void;
		handleNewItem: () => void;
		children?: Snippet;
	}>();

	let selectedId = $state<string | null>(null);

	let createNewItem = $state<boolean>(false);

	function success(successType: SuccessType) {
		successResponse(successType);
		// if (successType === SuccessType.updatedName) console.log('Update success');
		// if (successType === SuccessType.deletedCharacter) console.log('Delete success');
	}

	function selected(id: string) {
		selectedId = id;
		handleSelectedItem(id);
	}

	function newItem() {
		createNewItem = true;
	}

	function save() {
		handleNewItem();
		createNewItem = false;
	}
</script>

<div class="flex flex-col">
	<!-- {#each items as item}
		<div class="w-full border-b-2 {item.id === selectedId && 'bg-slate-400'}">
			<EditableItem subtitle={item.subtitle} {selected} id={item.id} name={item.name} {success} />
		</div>
	{/each} -->

	{#if createNewItem}
		<div class="w-full border-b-2">
			{#if children}
				{@render children()}
			{/if}
		</div>
		<div class="mt-1 flex justify-end">
			<TextButton text={'Save'} onClick={save} />
		</div>
	{:else}
		<div class="mt-1 flex justify-end">
			<Button buttonType={ButtonType.add} iconType={IconType.add} onClick={newItem} />
		</div>
	{/if}
</div>
