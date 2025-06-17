using System.Globalization;
using CsvHelper;
using SkeletonLabRpg.Common.AILearning.Models;

namespace SkeletonLabRpg.Common.Extensions;

public static class CharacterAttributesAiExtensions
{
    public static Stream ConvertToCsv(this CharacterAttributesAi attribute)
    {
        var memoryStream = new MemoryStream();
        var success = false;
        try
        {
            using var textWriter = new StreamWriter(memoryStream, leaveOpen: true);
            using var csvHelper = new CsvWriter(textWriter, culture: CultureInfo.InvariantCulture);
            csvHelper.WriteRecord(attribute);
            textWriter.Flush();
            memoryStream.Position = 0;
            success = true;
            return memoryStream;
        }
        finally
        {
            if (!success)
            {
                memoryStream.Dispose();
            }
        }
    }

    public static bool DoesItemExistInCharacterAttributesAiList(this CharacterAttributesAi item, IEnumerable<CharacterAttributesAi> characterAttributes)
    {
        return characterAttributes.All(characterAttribute => 
            characterAttribute.Advantage == item.Advantage
            && characterAttribute.CriticalHit == item.CriticalHit
            && characterAttribute.AverageDamage == item.AverageDamage
            && characterAttribute.CharismaModifier == item.CharismaModifier
            && characterAttribute.DexterityModifier == item.DexterityModifier
            && characterAttribute.IntelligenceModifier == item.IntelligenceModifier
            && characterAttribute.ConstitutionModifier == item.ConstitutionModifier
            && characterAttribute.StrengthModifier == item.StrengthModifier
            && characterAttribute.WisdomModifier == item.WisdomModifier
            && characterAttribute.ClassType == item.ClassType
            && characterAttribute.DamageType == item.DamageType
            && characterAttribute.MinDamage == item.MinDamage
            && characterAttribute.MaxDamage == item.MaxDamage
            && characterAttribute.DieSize == item.DieSize
            && characterAttribute.NumDice == item.NumDice
            && characterAttribute.ExtraAttack == item.ExtraAttack
            && characterAttribute.Level == item.Level
            && characterAttribute.WeaponType == item.WeaponType
            && characterAttribute.WeaponBonus == item.WeaponBonus);
    }
}