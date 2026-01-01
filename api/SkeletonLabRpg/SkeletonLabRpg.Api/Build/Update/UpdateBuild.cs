using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Authorisation;
using SkeletonLabRpg.Api.Build.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models.Build;

namespace SkeletonLabRpg.Api.Build.Update;

public static class UpdateBuild
{
    private record Request(string Name, string Reason, string Template);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildEndpoints.UpdateRpgBuild, Handler);
        }

        private static async Task<IResult> Handler(
            [FromBody] Request request,
            [FromRoute] Guid id,
            [FromServices] AccountDetails accountDetails,
            [FromServices] IRepository<BuildModel> repository,
            ITaskCache<BuildModel> cache)
        {
            var build = await repository.GetById(id);
            
            if (build is null)
            {
                return Results.NotFound();
            }

            build.Name = request.Name;
            build.Template = request.Template;
            build.Reason = request.Reason;
            
            var result = await repository.Update(build);
            cache.Invalidate(accountDetails.Email);
            
            return Results.Ok(result);
        }
    }
}