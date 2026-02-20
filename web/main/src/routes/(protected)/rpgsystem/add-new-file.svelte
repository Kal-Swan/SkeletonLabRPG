<script lang="ts">
	import IconButton from '@components/icon-button.svelte';
	import Label from '@components/label.svelte';
	import { ButtonType, IconType } from '@models/Icon-type';

	type AddNewFileProps = {
		openNewFile: (file: File) => void;
		removeFile: (fileName: string) => void;
		rpgSystem: {
			id?: string;
			name: string;
			fileNames?: string[];
			files?: File[] | undefined;
		};
	};

	let { rpgSystem, openNewFile, removeFile }: AddNewFileProps = $props();
</script>

{#if (rpgSystem?.files ?? []).length > 0}
	<Label text="Files added:" textSize="sm" />
{/if}

<div class="grid w-full grid-cols-1 gap-2 md:grid-cols-3">
	{#each rpgSystem.files as file, index (index)}
		<div class="mb-2 flex justify-between sm:w-full sm:justify-start md:items-center md:gap-2">
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
