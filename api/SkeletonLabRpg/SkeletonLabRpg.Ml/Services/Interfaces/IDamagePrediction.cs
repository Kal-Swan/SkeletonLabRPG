using SkeletonLabRpg.Ml.Models;

namespace SkeletonLabRpg.Ml.Services.Interfaces;

public interface IDamagePrediction
{
    (CharacterAttributesTrainingModel Input, float Prediction) PredictDamage(IEnumerable<CharacterAttributesTrainingModel> attributesTrainingModels);
}