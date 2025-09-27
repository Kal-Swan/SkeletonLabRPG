<script lang="ts">
	let { text, textSize, isHeader } = $props<{
		text: string;
		textSize?: string;
		isHeader?: boolean;
	}>();
</script>

<div
	class="{textSize ? textSize : 'text-md'} {isHeader
		? 'mb-5'
		: ''} text-gray-500 opacity-80 dark:text-gray-400"
>
	{text}
</div>
