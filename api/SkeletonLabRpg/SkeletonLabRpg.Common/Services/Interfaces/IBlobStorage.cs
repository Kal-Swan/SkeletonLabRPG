using Azure.Storage.Blobs;

namespace SkeletonLabRpg.Common.Services.Interfaces;

public interface IBlobStorage
{
    Task UploadBlobAsync(string containerName, string blobName, Stream data);

    Task<Stream?> DownloadBlobAsync(string containerName, string blobName);

    BlobClient GetBlobClient(string containerName, string blobName);
}