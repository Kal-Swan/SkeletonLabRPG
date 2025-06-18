using SkeletonLabRpg.Common.AILearning.Models;

namespace SkeletonLabRpg.Ml.Services.Interfaces;

public interface IDamageModelTrainer
{
    void TrainModel(IEnumerable<CharacterAttributesAi> characterAttributesAis, Stream streamToSaveto);
}