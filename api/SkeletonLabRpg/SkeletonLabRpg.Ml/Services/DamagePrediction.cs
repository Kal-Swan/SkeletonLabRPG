using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ML;
using SkeletonLabRpg.Common.Exceptions;
using SkeletonLabRpg.Ml.Models;
using SkeletonLabRpg.Ml.Services.Interfaces;

namespace SkeletonLabRpg.Ml.Services;

public class DamagePrediction(
    PredictionEnginePool<CharacterAttributesTrainingModel, Models.DamagePrediction> predictionEnginePool,
    ILogger<DamagePrediction> logger)
    : IDamagePrediction
{
    public (CharacterAttributesTrainingModel Input, float Prediction) PredictDamage(IEnumerable<CharacterAttributesTrainingModel> attributesTrainingModels)
    {
        try
        {
            var highestDamageBuild = attributesTrainingModels
                .Select(candidate => new
                {
                    Input = candidate,
                    Prediction = predictionEnginePool.Predict(candidate).PredictedAverageDamage
                })
                .OrderByDescending(result => result.Prediction)
                .FirstOrDefault();

            if (highestDamageBuild is null)
            {
                logger.LogInformation("Prediction engine returned null");
                throw new MachineLearningException("Failed to predict damage");
            }

            if (highestDamageBuild.Prediction <= 0)
            {
                logger.LogInformation("Prediction engine returned: {Prediction}", highestDamageBuild.Prediction);
                throw new MachineLearningException("Failed to predict damage");
            }
            return (highestDamageBuild.Input, highestDamageBuild.Prediction);
        }
        catch (Exception exception)
        {
            logger.LogError("Prediction engine failed with exception: {Exception}", exception);
            throw new MachineLearningException("Failed to predict damage");
        }
    }
}