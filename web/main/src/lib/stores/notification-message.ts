import type { Notification } from '@models/notification.types';
import { writable } from 'svelte/store';

export const notificationMessages = writable<Notification[]>();
