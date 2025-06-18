using SkeletonLabRpg.Common.Database.Cosmosdb.Entities;
using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Database.Models;

public class CharacterAttributes : CosmosDbDocumentBase<CharacterAttributes>
{
    public int Level { get; set; }
    public int NumDice { get; set; }
    public int DieSize { get; set; }
    public WeaponType WeaponType { get; set; }
    public int StrengthModifier { get; set; }
    public int DexterityModifier { get; set; }
    public int ConstitutionModifier { get; set; }
    public int IntelligenceModifier { get; set; }
    public int WisdomModifier { get; set; }
    public int CharismaModifier { get; set; }
    public int WeaponBonus { get; set; }
    public bool CriticalHit { get; set; }
    public DamageType DamageType { get; set; }
    public int ExtraAttack { get; set; }
    
    public ClassType ClassType { get; set; }
    public int MinDamage { get; set; }
    public int MaxDamage { get; set; }
    public float AverageDamage { get; set; }
    public Guid CharacterClassId { get; set;  }
}