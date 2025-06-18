using SkeletonLabRpg.Common.Database.Enums;

namespace SkeletonLabRpg.Common.Extensions;

public static class DamageTypeExtensions
{
    public static DamageType? ToDamageType(this string value)
    {
        if (Enum.TryParse(value, out DamageType damageType))
        {
            return damageType;
        }

        return null;
    }
}