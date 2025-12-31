export const Actions = {
	updateBuildRequest: 'updateBuildRequest',
	createBuildRequest: 'createBuildRequest'
};

export type ActionType = (typeof Actions)[keyof typeof Actions];
