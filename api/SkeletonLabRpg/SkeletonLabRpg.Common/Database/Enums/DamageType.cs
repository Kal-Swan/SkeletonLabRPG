using System.ComponentModel;

namespace SkeletonLabRpg.Common.Database.Enums;

public enum DamageType
{
    [Description("Acid")]
    Acid = 1,
    [Description("Bludgeoning")]
    Bludgeoning = 2,
    [Description("Cold")]
    Cold = 3,
    [Description("Fire")]
    Fire = 4,
    [Description("Force")]
    Force = 5,
    [Description("Lightning")]
    Lightning = 6,
    [Description("Necrotic")]
    Necrotic = 7,
    [Description("Piercing")]
    Piercing = 8,
    [Description("Poison")]
    Poison = 9,
    [Description("Psychic")]
    Psychic = 10,
    [Description("Radiant")]
    Radiant = 11,
    [Description("Slashing")]
    Slashing = 12,
    [Description("Thunder")]
    Thunder = 13
}