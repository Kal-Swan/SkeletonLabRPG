import type { CharacterAttributeDamage } from '@models/character/character-attribute-schema';

export interface CharacterAttributeRequest {
	favourite: CharacterAttributeDamage | null;
	others: CharacterAttributeDamage[];
}
