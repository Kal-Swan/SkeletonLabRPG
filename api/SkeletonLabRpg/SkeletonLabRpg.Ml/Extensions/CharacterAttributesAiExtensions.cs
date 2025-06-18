using SkeletonLabRpg.Common.AILearning.Models;
using SkeletonLabRpg.Ml.Models;

namespace SkeletonLabRpg.Ml.Extensions;

public static class CharacterAttributesAiExtensions
{
    public static CharacterAttributesTrainingModel ConvertToAttributeTrainingModel(this CharacterAttributesAi attributes)
    {
        return new CharacterAttributesTrainingModel
        {
            Level = attributes.Level,
            NumDice = attributes.NumDice,
            DieSize = attributes.DieSize,
            WeaponType = attributes.WeaponType.ToString(),
            StrengthModifier = attributes.StrengthModifier,
            DexterityModifier = attributes.DexterityModifier,
            ConstitutionModifier = attributes.ConstitutionModifier,
            IntelligenceModifier = attributes.IntelligenceModifier,
            WisdomModifier = attributes.WisdomModifier,
            CharismaModifier = attributes.CharismaModifier,
            WeaponBonus = attributes.WeaponBonus,
            CriticalHit = attributes.CriticalHit,
            DamageType = attributes.DamageType.ToString(),
            ExtraAttack = attributes.ExtraAttack,
            MinDamage = attributes.MinDamage,
            MaxDamage = attributes.MaxDamage,
            AverageDamage = attributes.AverageDamage,
            ClassType = attributes.ClassType.ToString(),
            CharacterTags = attributes.CharacterTags
        };
    } 
}