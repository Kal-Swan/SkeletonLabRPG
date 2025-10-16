<script lang="ts">
	import Icon from '@iconify/svelte';
	import { notificationMessages } from '@lib/stores/notification-message';
	import { NotificationType, type Notification } from '@models/notification.types';
	import '@fontsource/roboto';
	import IconButton from './icon-button.svelte';
	import { ButtonType } from '@models/Icon-type';
	let notifications = $state<Notification[]>([]);
	const onClose = (id: string) => {
		notifications = notifications.filter((notification) => notification.id !== id);
	};

	notificationMessages.subscribe((value) => {
		if (value) {
			notifications = [...notifications, ...value];
		}
	});

	const getBackgroundColor = (type: NotificationType) => {
		switch (type) {
			case NotificationType.Info:
				return '--info';
			case NotificationType.Success:
				return '--success';
			case NotificationType.Warning:
				return '--warning';
			case NotificationType.Error:
				return '--error';
			default:
				return '--info';
		}
	};
</script>

{#if notifications.length > 0}
	<div class="fixed top-0 right-0 z-10 w-full p-4 md:w-1/2 lg:w-1/3">
		{#each notifications as notification}
			<div
				style="background-color: var({getBackgroundColor(notification.type)});"
				class="notification mt-2 flex w-full justify-between rounded-md p-4 pt-3"
			>
				<div class="flex flex-col justify-end">
					{notification.message}
				</div>
				<IconButton
					buttonType={ButtonType.cancel}
					iconType="close-circle-outline"
					onClick={() => onClose(notification.id)}
					color="#1e1e2e"
				/>
				<!-- <Icon
					class="icon cursor-pointer hover:opacity-70"
					onclick={() => onClose(notification.id)}
					icon="solar:close-circle-line-duotone"
					width="24"
					height="24"
				/> -->
			</div>
		{/each}
	</div>
{/if}

<style>
	.notification {
		/* border: 0.5px solid black; */
		color: black;
		font-family: 'Roboto';
		font-size: 16px;
	}
</style>
