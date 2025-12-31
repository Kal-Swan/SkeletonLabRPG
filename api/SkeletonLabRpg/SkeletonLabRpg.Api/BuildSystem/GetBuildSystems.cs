using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class GetBuildSystems
{
    private record Response(Guid Id, string Name);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(BuildSystemEndpoints.Base, Handler);
        }

        private static async Task<IResult> Handler(
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<BuildSystemModel> repository)
        {
            var rpgSystems = await repository.GetMany(system => 
                system.AccountEmail == accountDetails.Email, accountDetails.Email);
            return Results.Ok(rpgSystems.Select(rpgSystem => new Response(rpgSystem.Id, rpgSystem.Name)));
        }
    }
}