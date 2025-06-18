import type { CharacterAttribute } from '@models/character/character-attribute-schema';
import type { Character } from '../models/character/character-attribute';
import type { DamageOutput } from '../models/damage-outputs';
import { WeaponType } from '../models/weapon-type';
import { ClassType } from '@models/class-type';
import { DamageType } from '@models/damage-type';

const spells = Object.values(DamageType)
	.filter(
		(value) =>
			value != DamageType.Bludgeoning &&
			value != DamageType.Slashing &&
			value != DamageType.Piercing
	)
	.map((value) => value as DamageType);

export function calculateDamage(character: CharacterAttribute): DamageOutput {
	let minDamage = character.numDice;
	let maxDamage = character.numDice * character.dieSize;
	let averageDamage = character.numDice * ((character.dieSize + 1) / 2);

	if (character.weaponType === WeaponType.Strength) {
		minDamage += modifierToDamage(character.strengthModifier);
		maxDamage += modifierToDamage(character.strengthModifier);
		averageDamage += modifierToDamage(character.strengthModifier);
	} else {
		minDamage += modifierToDamage(character.dexterityModifier);
		maxDamage += modifierToDamage(character.dexterityModifier);
		averageDamage += modifierToDamage(character.dexterityModifier);
	}

	const intelligenceClasses = [ClassType.Wizard, ClassType.Artificer];

	if (
		spells.includes(character.damageType) &&
		intelligenceClasses.includes(character.classType) &&
		character.intelligenceModifier > 0
	) {
		minDamage += character.intelligenceModifier;
		maxDamage += character.intelligenceModifier;
		averageDamage += character.intelligenceModifier;
	}

	const charismaClasses = [
		ClassType.Paladin,
		ClassType.Sorcerer,
		ClassType.Bard,
		ClassType.Warlock
	];

	if (
		spells.includes(character.damageType) &&
		charismaClasses.includes(character.classType) &&
		character.charismaModifier > 0
	) {
		minDamage += character.charismaModifier;
		maxDamage += character.charismaModifier;
		averageDamage += character.charismaModifier;
	}

	if (character.criticalHit) {
		minDamage *= 2;
		maxDamage *= 2;
		averageDamage *= 2;
	}

	if (character.extraAttack > 0) {
		minDamage *= character.extraAttack + 1;
		maxDamage *= character.extraAttack + 1;
		averageDamage *= character.extraAttack + 1;
	}

	if (character.weaponBonus > 0) {
		minDamage += character.weaponBonus;
		maxDamage += character.weaponBonus;
		averageDamage += character.weaponBonus;
	}

	return {
		maxDamage,
		minDamage,
		averageDamage
	};
}

function modifierToDamage(modifier: number): number {
	return Math.floor((modifier - 10) / 2);
}
