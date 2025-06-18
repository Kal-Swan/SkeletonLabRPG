import { ClassType } from '@models/class-type';
import { DamageType } from '@models/damage-type';
import { WeaponType } from '@models/weapon-type';
import { z } from 'zod';

export const CharacterAttributeSchema = z.object({
	level: z.number().int().min(1).max(20),
	numDice: z.number().int().min(1).max(10),
	dieSize: z.number().int().min(1).max(100),
	strengthModifier: z.number().int().min(8).max(26),
	dexterityModifier: z.number().int().min(8).max(26),
	constitutionModifier: z.number().int().min(8).max(26),
	intelligenceModifier: z.number().int().min(8).max(26),
	wisdomModifier: z.number().int().min(8).max(26),
	charismaModifier: z.number().int().min(8).max(26),
	weaponBonus: z.number().int().min(-5).max(16),
	weaponType: z.nativeEnum(WeaponType),
	criticalHit: z.boolean(),
	advantage: z.boolean(),
	extraAttack: z.number().int().min(0).max(10),
	damageType: z.nativeEnum(DamageType),
	classType: z.nativeEnum(ClassType),
	characterClassId: z.string().min(1)
});

export const CharacterDamageSchema = z.object({
	minDamage: z.number().int().min(1),
	maxDamage: z.number().int().min(1),
	averageDamage: z.number().min(1)
});

export const CharacterAttributeDamageSchema = CharacterAttributeSchema.merge(CharacterDamageSchema);

export type CharacterAttribute = z.infer<typeof CharacterAttributeSchema>;
export type CharacterDamage = z.infer<typeof CharacterDamageSchema>;
export type CharacterAttributeDamage = z.infer<typeof CharacterAttributeDamageSchema>;

//     name: string;
//     level: number;
//     baseDamage: number;
//     numDice: number;
//     dieSize: number;
//     weaponType: WeaponType;
//     strengthModifier: number;
//     dexterityModifier: number;
//     constitutionModifier: number;
//     intelligenceModifier: number;
//     wisdomModifier: number;
//     charismaModifier: number;
//     weaponBonus: number;
//     criticalHit: boolean;
//     advantage: boolean;
//     damageType: string; // Type of damage (e.g., slashing, piercing, fire)
//     extraAttack: number;
// }
