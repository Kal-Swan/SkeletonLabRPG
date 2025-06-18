using SkeletonLabRpg.Common.AILearning.Models;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Database.Models;

namespace SkeletonLabRpg.Common.Extensions;

public static class CharacterAttributesExtensions
{
    public static CharacterAttributesAi AssignCharacterTagsForAi(this CharacterAttributes characterAttributes)
    {
        var characterAttributesAi = new CharacterAttributesAi
        {
            ClassType = characterAttributes.ClassType,
            Level = characterAttributes.Level,
            NumDice = characterAttributes.NumDice,
            DieSize = characterAttributes.DieSize,
            WeaponType = characterAttributes.WeaponType,
            StrengthModifier = characterAttributes.StrengthModifier,
            DexterityModifier = characterAttributes.DexterityModifier,
            ConstitutionModifier = characterAttributes.ConstitutionModifier,
            IntelligenceModifier = characterAttributes.IntelligenceModifier,
            WisdomModifier = characterAttributes.WisdomModifier,
            CharismaModifier = characterAttributes.CharismaModifier,
            WeaponBonus = characterAttributes.WeaponBonus,
            CriticalHit = characterAttributes.CriticalHit,
            DamageType = characterAttributes.DamageType,
            ExtraAttack = characterAttributes.ExtraAttack,
            MinDamage = characterAttributes.MinDamage,
            MaxDamage = characterAttributes.MaxDamage,
            AverageDamage = characterAttributes.AverageDamage
        };
        
        if (ClassTags.ClassTagsMap.TryGetValue(characterAttributesAi.ClassType, out var tagsToAdd))
        {
            characterAttributesAi.CharacterTags.AddRange(tagsToAdd);
        }

        return characterAttributesAi;
    }
}