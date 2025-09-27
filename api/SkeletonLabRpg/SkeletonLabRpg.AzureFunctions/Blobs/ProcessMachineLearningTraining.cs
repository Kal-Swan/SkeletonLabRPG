using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Extensions;
using SkeletonLabRpg.Common.Services.Interfaces;

namespace SkeletonLabRpg.AzureFunctions.Blobs;

public class ProcessMachineLearningTraining(ILogger<ProcessMachineLearningTraining> logger, IBlobStorage blobStorage)
{
    [Function(nameof(ProcessMachineLearningTraining))]
    public async Task Run([BlobTrigger("character-attribute-models/characterattributemodels", Connection = "AzureWebJobsStorage")] string csv, string name)
    {
        // using var blobStreamReader = new StreamReader(stream);
        // var content = await blobStreamReader.ReadToEndAsync();
        // var records = csv.ConvertFromCsv();
        // var blobClient = blobStorage.GetBlobClient(BlobConstants.CharacterAttributeMlTrainingContainer, BlobConstants.CharacterAttributeMlTrainingFileName);
        // await using var blobStream = await blobClient.OpenWriteAsync(overwrite: true);
        // damageModelTrainer.TrainModel(records, blobStream);
        // logger.LogInformation($"C# Blob trigger function Processed blob\n Name: {name} \n Data: {content}");
    }
}