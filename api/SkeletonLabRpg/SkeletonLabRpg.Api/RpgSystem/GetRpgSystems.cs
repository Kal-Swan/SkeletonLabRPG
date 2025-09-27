using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Llm.Constants;
using SkeletonLabRpg.Api.RpgSystem.Constants;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;

namespace SkeletonLabRpg.Api.RpgSystem;

public static class GetRpgSystems
{
    private record Response(Guid Id, string Name);
    
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder app)
        {
            app.MapGet(RpgSystemEndpoints.Base, Handler);
        }

        private static async Task<IResult> Handler(
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<RpgSystemModel> repository)
        {
            var rpgSystems = await repository.GetMany(system => system.AccountEmail == accountDetails.Email);
            return Results.Ok(rpgSystems.Select(rpgSystem => new Response(rpgSystem.Id, rpgSystem.Name)));
        }
    }
}