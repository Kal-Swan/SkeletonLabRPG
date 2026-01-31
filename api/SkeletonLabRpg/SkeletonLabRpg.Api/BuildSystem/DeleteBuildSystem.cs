using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Cosmosdb.Enums;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Database.Models.Builds;
using SkeletonLabRpg.Common.Exceptions;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class DeleteBuildSystem
{
    //Not currently using, just here for completeness, will come back to this later
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapDelete(BuildSystemEndpoints.Delete, Handler);
        }
        
        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromServices] UserScopedRepository<BuildSystemModel> repository,
            [FromServices] UserScopedRepository<BuildRequestModel> buildRequestRepository,
            [FromServices] UserScopedRepository<BuildModel> buildRepository,
            [FromServices] IBlobStorage blobStorage,
            [FromServices] AccountDetails accountDetails)
        {
            var current = await repository.GetById(id);

            if (current is null)
            {
                throw new NotFoundException("Build System already deleted", showCustomMessage: true);
            }

            current.Status = DocumentStatus.Deleting;
            current.Deleted = DateTime.UtcNow;
            
            var deleting = await repository.Update(current);
            
            foreach (var fileName in current.FileNames)
            {
                await blobStorage.DeleteBlobAsync(
                    BlobStorageConstants.UserBuildSystemContainer,
                    $"{deleting.Id}/{fileName}");
            }
            
            await buildRequestRepository.DeleteMany(
                br => br.BuildSystemId == deleting.Id);
            
            await buildRepository.DeleteMany(
                b => b.BuildSystemId == deleting.Id);
            
            var deleted = await repository.Delete(deleting.Id);
            
            return Results.Ok(deleted);
        }
    }
}