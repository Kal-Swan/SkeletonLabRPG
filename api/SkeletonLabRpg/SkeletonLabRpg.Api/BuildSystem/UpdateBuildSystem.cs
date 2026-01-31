using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.BuildSystem.Constants;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Common.Authorisation;
using SkeletonLabRpg.Common.Cache;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Database.Cosmosdb;
using SkeletonLabRpg.Common.Database.Models.Build;
using SkeletonLabRpg.Common.Exceptions;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Api.BuildSystem;

public static class UpdateBuildSystem
{
    private record Request(string Name, [FromForm] List<IFormFile> Files);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildSystemEndpoints.Update, Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            [FromBody] Request request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] UserScopedRepository<BuildSystemModel> repository,
            [FromServices] IBlobStorage blobStorage)
        {
            var current = await repository.GetById(id);

            if (current is null)
            {
                throw new NotFoundException("Build System not found", showCustomMessage: true);
            }

            current.Name = request.Name;
            current.FileNames = request.Files.Select(file => file.FileName);
            
            foreach (var file in request.Files)
            {
                await using var stream = file.OpenReadStream();
                await blobStorage.UploadBlobAsync(
                    BlobStorageConstants.UserBuildSystemContainer, 
                    $"{current.Id}/{file.FileName}",
                    stream, file.ContentType);
            }
            
            var updated = await repository.Update(current);
            return Results.Ok(updated);
        }
    }
}