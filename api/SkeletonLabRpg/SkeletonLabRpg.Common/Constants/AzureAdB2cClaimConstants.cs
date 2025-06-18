using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Constants;

public static class AzureAdB2CClaimConstants
{
    public const string Role = "role";
    public static readonly string[] Roles =
    [
        nameof(RoleType.CharacterFullAccess),
        nameof(RoleType.PredictCreate)
    ];
}