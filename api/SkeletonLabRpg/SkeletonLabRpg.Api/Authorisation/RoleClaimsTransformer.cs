// using System.Security.Claims;
// using DEXRPG.WebApi.Extensions;
// using Microsoft.AspNetCore.Authentication;
//
// namespace DEXRPG.WebApi.Authorisation;
//
// public class RoleClaimsTransformer : IClaimsTransformation
// {
//     private const string RoleName = "role";
//
//     public Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
//     {
//         var identity = (ClaimsIdentity?)principal.Identity;
//
//         if (identity is null)
//         {
//             return Task.FromResult(principal);
//         }
//
//         var roles = principal.Claims.GetClaimsByType(RoleName).ToList();
//
//         foreach (var role in roles)
//         {
//             if (!identity.HasClaim(RoleName, role))
//             {
//                 identity.AddClaim(new Claim(ClaimTypes.Role, role));
//             }
//         }
//
//         return Task.FromResult(principal);
//     }
// }