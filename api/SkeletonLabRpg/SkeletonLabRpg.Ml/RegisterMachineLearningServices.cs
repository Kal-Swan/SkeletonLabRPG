using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.ML;
using SkeletonLabRpg.Ml.Models;
using SkeletonLabRpg.Ml.Services;
using SkeletonLabRpg.Ml.Services.Interfaces;
using DamagePrediction = SkeletonLabRpg.Ml.Services.DamagePrediction;

namespace SkeletonLabRpg.Ml;

public static class RegisterMachineLearningServices
{
    public static IServiceCollection ConfigureMachineLearningServices(this IServiceCollection services)
    {
        services.AddPredictionEnginePool<CharacterAttributesTrainingModel, Models.DamagePrediction>();
        services.AddTransient<IDamageModelTrainer, DamageModelTrainer>();
        services.AddTransient<IDamagePrediction, DamagePrediction>();
        return services;
    }
}