using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.RpgBuild.Constants;
using SkeletonLabRpg.Api.RpgBuild.Extensions;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.RpgBuild;

namespace SkeletonLabRpg.Api.RpgBuild.GetAll;

public static class GetAllRpgBuilds
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapGet(BuildEndpoints.GetAllRpgBuilds, Handler);
        }

        private static async Task<IResult> Handler(
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<Build> buildRepository,
            [FromServices] IRepository<RpgSystemModel> rpgSystemRepository)
        {
            var builds = await buildRepository.GetMany(build => build.AccountEmail == accountDetails.Email);
            var rpgSystems = await rpgSystemRepository.GetMany(system => system.AccountEmail == accountDetails.Email);
            
            var groupBuilds = builds
                .Select(build => build.ToBuildRequest())
                .GroupBy(build => build.RpgSystemId)
                .ToDictionary(group => rpgSystems.First(system => system.Id == group.Key).Name, group => group.ToList());
            
            return Results.Ok(groupBuilds);
        }
    }
}