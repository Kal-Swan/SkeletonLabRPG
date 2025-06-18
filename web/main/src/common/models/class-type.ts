export enum ClassType {
	Fighter = 1,
	Barbarian = 2,
	Rogue = 3,
	Ranger = 4,
	Monk = 5,
	Paladin = 6,
	Cleric = 7,
	Druid = 8,
	Wizard = 9,
	Sorcerer = 10,
	Bard = 11,
	Warlock = 12,
	Artificer = 13
}

export const ClassTypeNameMap: Record<ClassType, string> = {
	[ClassType.Fighter]: 'Fighter',
	[ClassType.Barbarian]: 'Barbarian',
	[ClassType.Rogue]: 'Rogue',
	[ClassType.Ranger]: 'Ranger',
	[ClassType.Monk]: 'Monk',
	[ClassType.Paladin]: 'Paladin',
	[ClassType.Cleric]: 'Cleric',
	[ClassType.Druid]: 'Druid',
	[ClassType.Wizard]: 'Wizard',
	[ClassType.Sorcerer]: 'Sorcerer',
	[ClassType.Bard]: 'Bard',
	[ClassType.Warlock]: 'Warlock',
	[ClassType.Artificer]: 'Artificer'
};

export const classTypeOptions = Object.values(ClassType)
	.filter((value) => typeof value !== 'string')
	.map((type) => ({
		value: type,
		label: ClassTypeNameMap[type]
	}));
