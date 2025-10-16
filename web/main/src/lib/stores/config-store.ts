import type { Configuration } from '@lib/configuration';
import { writable } from 'svelte/store';

export const configStore = writable<Configuration>();
