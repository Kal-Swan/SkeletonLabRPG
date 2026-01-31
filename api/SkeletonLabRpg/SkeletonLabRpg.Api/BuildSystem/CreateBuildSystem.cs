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

public static class CreateBuildSystem
{
    private record Response(Guid Id, string Name, IEnumerable<string> FileNames);
    public class ApiEndpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(BuildSystemEndpoints.Base, Handler)
                .DisableAntiforgery();
        }

        private static async Task<IResult> Handler(
            HttpRequest request,
            [FromServices] AccountDetails accountDetails,
            [FromServices] UserScopedRepository<BuildSystemModel> repository,
            [FromServices] IMemoryCache<BuildSystemModel> cache,
            [FromServices] IBlobStorage blobStorage)
        {
            var form = await request.ReadFormAsync();
            var name = form["name"].ToString();
            var files = form.Files;
            
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new BadRequestException("Rpg System Name is required", showCustomMessage: true);
            }
            
            if (!files.Any(file => FileValidation.AllowedFileTypes.Contains(file.ContentType)))
            {
                throw new BadRequestException("Invalid file type, only text and pdf is allowed", showCustomMessage: true);
            }
            
            var system = new BuildSystemModel
            {
                Name = name,
                FileNames = files.Select(file => file.FileName)
            };
            
            foreach (var file in files)
            {
                await using var stream = file.OpenReadStream();
                await blobStorage.UploadBlobAsync(
                    BlobStorageConstants.UserBuildSystemContainer, 
                    $"{accountDetails.UserId}/{system.Id}/{file.FileName}",
                    stream, file.ContentType);
            }
            
            var result = await repository.Create(system);
            
            cache.Invalidate(accountDetails.UserId.ToString());
            
            return Results.Ok(new Response(result.Id, result.Name, result.FileNames));
        }
    } 
}