using SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Models;
using SkeletonLabRpg.Common.Database.Models;

namespace SkeletonLabRpg.Api.Character.CharacterClass.CharacterAttribute.Extensions;

public static class CharacterAttributeRequestExtensions
{
    public static CharacterAttributes ToCharacterAttributesModel(this CharacterAttributeRequest request, string accountEmail, Guid? characterClassId = null)
    {
        var characterAttributes = new CharacterAttributes
        {
            Level = request.Level,
            NumDice = request.NumDice,
            DieSize = request.DieSize,
            WeaponType = request.WeaponType,
            StrengthModifier = request.StrengthModifier,
            DexterityModifier = request.DexterityModifier,
            ConstitutionModifier = request.ConstitutionModifier,
            IntelligenceModifier = request.IntelligenceModifier,
            WisdomModifier = request.WisdomModifier,
            CharismaModifier = request.CharismaModifier,
            WeaponBonus = request.WeaponBonus,
            CriticalHit = request.CriticalHit,
            DamageType = request.DamageType,
            ExtraAttack = request.ExtraAttack,
            MinDamage = request.MinDamage,
            MaxDamage = request.MaxDamage,
            AverageDamage = request.AverageDamage,
            AccountEmail = accountEmail
        };
        
        if (characterClassId.HasValue)
        {
            characterAttributes.CharacterClassId = characterClassId.Value;
        }
        return characterAttributes;
    }
}