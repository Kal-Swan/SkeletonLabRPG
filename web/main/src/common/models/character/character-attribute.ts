import type { WeaponType } from '../weapon-type';

export interface Character {
	name: string;
	level: number;
	baseDamage: number;
	numDice: number;
	dieSize: number;
	weaponType: WeaponType;
	strengthModifier: number;
	dexterityModifier: number;
	constitutionModifier: number;
	intelligenceModifier: number;
	wisdomModifier: number;
	charismaModifier: number;
	weaponBonus: number;
	criticalHit: boolean;
	advantage: boolean;
	damageType: string; // Type of damage (e.g., slashing, piercing, fire)
	extraAttack: number;
}
