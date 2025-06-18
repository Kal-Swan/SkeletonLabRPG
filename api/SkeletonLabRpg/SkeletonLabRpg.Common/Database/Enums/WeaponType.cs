using System.ComponentModel;

namespace SkeletonLabRpg.Common.Database.Enums;

public enum WeaponType
{
    [Description("None")]
    None = 0,
    [Description("Greatsword")]
    Greatsword = 1,
    [Description("Longsword")]
    Longsword = 2,
    [Description("Rapier")]
    Rapier = 3,
    [Description("Hand Crossbow")]
    HandCrossbow = 4,
    [Description("Heavy Crossbow")]
    HeavyCrossbow = 6,
    [Description("Longbow")]
    Longbow = 7,
    [Description("Dagger")]
    Dagger = 8,
    [Description("Light Crossbow")]
    LightCrossbow = 9,
    [Description("Scimitar")]
    Scimitar = 10,
    [Description("Greataxe")]
    Greataxe = 11,
    [Description("Handaxe")]
    Handaxe = 12
}