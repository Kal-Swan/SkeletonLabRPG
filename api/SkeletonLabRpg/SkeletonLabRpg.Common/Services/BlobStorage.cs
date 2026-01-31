using System.Net;
using Azure;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.Extensions.Logging;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.Common.Services;

public class BlobStorage(BlobServiceClient blobServiceClient, ILogger<BlobStorage> logger) : IBlobStorage
{

    public BlobClient GetBlobClient(string containerName, string blobName)
    {
        var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
        return containerClient.GetBlobClient(blobName);
    }
    
    public async Task UploadBlobAsync(string containerName, string blobName, Stream data, string contentType)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            if (!await containerClient.ExistsAsync())
            {
                await containerClient.CreateAsync();
            }
            
            var blobClient = containerClient.GetBlobClient(blobName);

            if (data.CanSeek)
            {
                data.Position = 0;
            }
            
            await blobClient.UploadAsync(data, overwrite: true);
            
            await blobClient.SetHttpHeadersAsync(new BlobHttpHeaders
            {
                ContentType = contentType,
                ContentDisposition = "inline"
            });
        }
        catch (RequestFailedException exception)
        {
            logger.LogError(exception, "Azure Storage failure during blob upload for blob '{BlobName}' in container '{ContainerName}'. Status: {Status}, ErrorCode: {ErrorCode}", blobName, containerName, exception.Status, exception.ErrorCode);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected error during blob upload for blob '{BlobName}' in container '{ContainerName}'", blobName, containerName);            
            throw;
        }
    }
    
    public async Task<(Stream Content, string ContentType)?> DownloadBlobAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            var response = await blobClient.DownloadAsync();

            return (response.Value.Content, response.Value.ContentType);
        }
        catch (RequestFailedException exception) when (exception.Status == (int)HttpStatusCode.NotFound)
        {
            logger.LogInformation("Failed to download data from blob storage: {Exception}", exception);
            return null;
        }
        catch (RequestFailedException exception)
        {
            logger.LogError(exception,
                "Azure Storage exception during blob download for blob '{BlobName}' in container '{ContainerName}'. Status: {Status}, ErrorCode: {ErrorCode}",
                blobName, containerName, exception.Status, exception.ErrorCode);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected error during blob download for blob '{BlobName}' in container '{ContainerName}'", blobName, containerName);
            throw;
        }
    }
    
    public async Task DeleteBlobAsync(string containerName, string blobName)
    {
        try
        {
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            var blobClient = containerClient.GetBlobClient(blobName);
            await blobClient.DeleteIfExistsAsync();
        }
        catch (RequestFailedException exception)
        {
            logger.LogError(exception,
                "Azure Storage exception during blob deletion for blob '{BlobName}' in container '{ContainerName}'. Status: {Status}, ErrorCode: {ErrorCode}",
                blobName, containerName, exception.Status, exception.ErrorCode);
            throw;
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Unexpected error during blob deletion for blob '{BlobName}' in container '{ContainerName}'", blobName, containerName);
            throw;
        }
    }
}