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
    private record Request(string Name, IEnumerable<string> FileNames)
    {
        public IEnumerable<IFormFile> Files { get; } = [];
    }

    private record Response(Guid Id, string Name, IEnumerable<string> FileNames);
    
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPut(BuildSystemEndpoints.Update, Handler);
        }

        private static async Task<IResult> Handler(
            [FromRoute] Guid id,
            HttpRequest request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] UserScopedRepository<BuildSystemModel> repository,
            [FromServices] IBlobStorage blobStorage)
        {
            var form = await request.ReadFormAsync();
            var name = form["name"].ToString();
            var fileNames = form["fileNames"].Select(fileName => fileName).ToArray();
            var files = form.Files;
            
            var current = await repository.GetById(id);

            if (current is null)
            {
                throw new NotFoundException("Build System not found", showCustomMessage: true);
            }

            if (files.Any())
            {
                foreach (var fileName in current.FileNames)
                {
                    await blobStorage.DeleteBlobAsync(BlobStorageConstants.UserBuildSystemContainer, current.Id, fileName);
                }
            }
            
            current.Name = name;
            current.FileNames = files.Select(file => file.FileName).Concat(fileNames);
            
            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();
                await blobStorage.UploadBlobAsync(
                    BlobStorageConstants.UserBuildSystemContainer, 
                    current.Id, file.FileName,
                    stream, file.ContentType);
            }
            
            var updated = await repository.Update(current);
            return Results.Ok(new Response(updated.Id, updated.Name, updated.FileNames));
        }
    }
}