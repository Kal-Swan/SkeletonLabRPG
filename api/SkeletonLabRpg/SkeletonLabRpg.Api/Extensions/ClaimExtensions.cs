using System.Security.Claims;

namespace SkeletonLabRpg.Api.Extensions;

public static class ClaimExtensions
{
    public static IEnumerable<string> GetRoles(this IEnumerable<Claim> claims)
    {
        return claims.Where(claim => claim.Type == ClaimTypes.Role).Select(claim => claim.Value);
    }
    
    public static IEnumerable<string> GetClaims(this IEnumerable<Claim> claims, string claimType)
    {
        return claims.Where(claim => claim.Type.EndsWith(claimType)).Select(claim => claim.Value);
    }
}