using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Constants;

public static class ClassTags
{
    public static readonly Dictionary<ClassType, List<string>> ClassTagsMap = new()
    {
        { ClassType.Wizard, ["DPS", "SpellCaster", "Arcane", "Magic", "LightArmor", "HighDamage", "Utility", "Elemental"] },
        { ClassType.Paladin, ["Tank", "Healer", "Support", "Divine", "DPS", "Holy", "Aura", "AuraBuffs", "Face"] },
        { ClassType.Ranger, ["DPS", "Ranged", "Nature", "Agility", "Survival", "Tracking", "Bow", "Versatile"] },
        { ClassType.Rogue, ["DPS", "Stealth", "Melee", "Ranged", "Agility", "SneakAttack", "Physical", "Debuff"] },
        { ClassType.Barbarian, ["DPS", "Melee", "Tank", "Strength", "Rage", "Brutal", "Physical", "Fury"] },
        { ClassType.Bard, ["Support", "SpellCaster", "Buffs", "Charm", "Versatile", "Magic", "Debuff", "Ranged", "Face"] },
        { ClassType.Cleric, ["Healer", "SpellCaster", "Support", "Divine", "Tank", "Buffs", "Healing", "Holy"] },
        { ClassType.Druid, ["SpellCaster", "Nature", "Shapeshift", "Healing", "Tank", "Support", "Elemental", "Versatile"] },
        { ClassType.Fighter, ["DPS", "Melee", "Tank", "Physical", "Armor", "Weapons", "Versatile", "Combat"] },
        { ClassType.Monk, ["Melee", "DPS", "Unarmored", "Agility", "MartialArts", "Speed", "Physical", "Ki"] },
        { ClassType.Sorcerer, ["DPS", "SpellCaster", "Magic", "Arcane", "Elemental", "HighDamage", "Flexibility", "Mana", "Face"] },
        { ClassType.Warlock, ["DPS", "SpellCaster", "Magic", "Arcane", "Cursed", "Pact", "Debuff", "Summoning"] },
        { ClassType.Artificer, ["Support", "SpellCaster", "Engineering", "MagicTech", "Versatile", "Crafting", "Ranged", "Healing", "Face"] }
    };
}