using Microsoft.ML.Data;

namespace SkeletonLabRpg.Ml.Models;

public class CharacterAttributesTrainingModel
{
    [LoadColumn(0)]
    public int Level { get; set; }
    
    [LoadColumn(1)]
    public int NumDice { get; set; }
    
    [LoadColumn(2)]
    public int DieSize { get; set; }
    
    [LoadColumn(3)]
    public string WeaponType { get; set; }
    
    [LoadColumn(4)]
    public int StrengthModifier { get; set; }
    
    [LoadColumn(5)]
    public int DexterityModifier { get; set; }
    
    [LoadColumn(6)]
    public int ConstitutionModifier { get; set; }
    
    [LoadColumn(7)]
    public int IntelligenceModifier { get; set; }
    
    [LoadColumn(8)]
    public int WisdomModifier { get; set; }
    
    [LoadColumn(9)]
    public int CharismaModifier { get; set; }
    
    [LoadColumn(10)]
    public int WeaponBonus { get; set; }
    
    [LoadColumn(11)]
    public bool CriticalHit { get; set; }
    
    [LoadColumn(12)]
    public string DamageType { get; set; }
    
    [LoadColumn(13)]
    public string ClassType { get; set; }
    
    [LoadColumn(14)]
    public int ExtraAttack { get; set; }
    
    [LoadColumn(15)]
    public IEnumerable<string> CharacterTags { get; set; } 
    
    [LoadColumn(16)]
    public int MinDamage { get; set; }
    
    [LoadColumn(17)]
    public int MaxDamage { get; set; }
    
    [LoadColumn(18)]
    public float AverageDamage { get; set; }
    
}