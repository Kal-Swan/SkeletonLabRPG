<script lang="ts">
	import type { ClassType } from '@models/class-type';
	import CharacterAttributeForm from './character-attribute-form.svelte';

	let { id, classType } = $props<{ id: string; classType: ClassType }>();
</script>

<div class="flex flex-col">
	{id}
	<CharacterAttributeForm {classType} {id} />
</div>
