using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.AILearning.Constants;

public static class CharacterDescriptions
{
    public static List<ClassType> MagicUser =>
    [
        ClassType.Wizard,
        ClassType.Sorcerer,
        ClassType.Cleric,
        ClassType.Druid
    ];
    
    public static List<ClassType> MeleeUser =>
    [
        ClassType.Fighter,
        ClassType.Barbarian,
        ClassType.Rogue,
        ClassType.Ranger,
        ClassType.Monk,
        ClassType.Paladin
    ];
    
    public static List<ClassType> RangedUser =>
    [
        ClassType.Rogue,
        ClassType.Ranger,
        ClassType.Monk,
        ClassType.Paladin
    ];
}