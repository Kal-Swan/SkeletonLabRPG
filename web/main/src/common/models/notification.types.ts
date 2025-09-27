export enum NotificationType {
	Info = 'info',
	Warning = 'warning',
	Success = 'success',
	Error = 'error'
}

export type Notification = {
	type: NotificationType;
	message: string;
	id: string;
};
