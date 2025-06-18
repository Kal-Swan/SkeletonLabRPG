using System.ComponentModel;

namespace SkeletonLabRpg.Common.Database.Enums;

public enum ClassType
{
    [Description("Fighter")]
    Fighter = 1,
    [Description("Barbarian")]
    Barbarian = 2,
    [Description("Rogue")]
    Rogue = 3,
    [Description("Ranger")]
    Ranger = 4,
    [Description("Monk")]
    Monk = 5,
    [Description("Paladin")]
    Paladin = 6,
    [Description("Cleric")]
    Cleric = 7,
    [Description("Druid")]
    Druid = 8,
    [Description("Wizard")]
    Wizard = 9,
    [Description("Sorcerer")]
    Sorcerer = 10,
    [Description("Bard")]
    Bard = 11,
    [Description("Warlock")]
    Warlock = 12,
    [Description("Artificer")]
    Artificer = 13,
}