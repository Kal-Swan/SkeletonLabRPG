using Microsoft.ML;
using SkeletonLabRpg.Common.AILearning.Models;
using SkeletonLabRpg.Ml.Extensions;
using SkeletonLabRpg.Ml.Services.Interfaces;

namespace SkeletonLabRpg.Ml.Services;

public class DamageModelTrainer : IDamageModelTrainer
{
    private const int Seed = 1;
    private readonly MLContext _mlContext = new(Seed);
    
    public void TrainModel(IEnumerable<CharacterAttributesAi> characterAttributesAis, Stream streamToSaveto)
    {
        var data = _mlContext.Data.LoadFromEnumerable(
            characterAttributesAis.Select(ai => ai.ConvertToAttributeTrainingModel()));
        
        var pipeline = _mlContext.Transforms.Conversion.
            MapValueToKey("Level", "LevelEncoded")
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("NumDice", "NumDiceEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("DieSize", "DieSizeEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("WeaponType", "WeaponTypeEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("StrengthModifier", "StrengthModifierEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("DexterityModifier", "DexterityModifierEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ConstitutionModifier", "ConstitutionModifierEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("IntelligenceModifier", "IntelligenceModifierEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("WisdomModifier", "WisdomModifierEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("CharismaModifier", "CharismaModifierEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("WeaponBonus", "WeaponBonusEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("CriticalHit", "CriticalHitEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("DamageType", "DamageTypeEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ExtraAttack", "ExtraAttackEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("ClassType", "ClassTypeEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("CharacterTags", "CharacterTagsEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("MinDamage", "MinDamageEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("MaxDamage", "MaxDamageEncoded"))
            .Append(_mlContext.Transforms.Conversion.MapValueToKey("AverageDamage", "AverageDamageEncoded"))
            .Append(_mlContext.Transforms.Concatenate("Features",
                "LevelEncoded",
                "NumDiceEncoded",
                "DieSizeEncoded",
                "WeaponTypeEncoded",
                "StrengthModifierEncoded",
                "DexterityModifierEncoded",
                "ConstitutionModifierEncoded",
                "IntelligenceModifierEncoded",
                "WisdomModifierEncoded",
                "CharismaModifierEncoded",
                "WeaponBonusEncoded",
                "CriticalHitEncoded",
                "DamageTypeEncoded",
                "ExtraAttackEncoded",
                "ClassTypeEncoded",
                "CharacterTagsEncoded",
                "MinDamageEncoded",
                "MaxDamageEncoded",
                "AverageDamageEncoded"
            ))
            .Append(_mlContext.Regression.Trainers.FastTree(labelColumnName: "AverageDamage", featureColumnName: "Features"));

        
        var model = pipeline.Fit(data);
        
        _mlContext.Model.Save(model, data.Schema, streamToSaveto);
    }

    public void TrainLightLbmModel(IEnumerable<CharacterAttributesAi> characterAttributesAis, Stream streamToSaveto)
    {
        var data = _mlContext.Data.LoadFromEnumerable(
            characterAttributesAis.Select(ai => ai.ConvertToAttributeTrainingModel()));

        var categoricalFeatures = new[]
        {
            "WeaponTypeEncoded",
            "DamageTypeEncoded",
            "CharacterTagsEncoded",
            "ClassTypeEncoded"
        };

        var numericFeatures = new[]
        {
            "Level",
            "NumDice",
            "DieSize",
            "StrengthModifier",
            "DexterityModifier",
            "ConstitutionModifier",
            "IntelligenceModifier",
            "WisdomModifier",
            "CharismaModifier",
            "WeaponBonus",
            "CriticalHit",
            "ExtraAttack",
            "MinDamage",
            "MaxDamage"
        };

        var allFeatures = categoricalFeatures.Concat(numericFeatures.Select(col => $"{col}_Norm")).ToArray();

        var preprocessingPipeline = _mlContext.Transforms
            .Conversion.MapKeyToValue("WeaponTypeEncoded", "WeaponType")
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("DamageTypeEncoded", "DamageType"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("CharacterTagsEncoded", "CharacterTags"))
            .Append(_mlContext.Transforms.Conversion.MapKeyToValue("ClassTypeEncoded", "ClassType"));

        foreach (var col in numericFeatures)
        {
            preprocessingPipeline.Append(_mlContext.Transforms.NormalizeMinMax($"{col}_Norm", col));
        }

        preprocessingPipeline.Append(_mlContext.Transforms.Concatenate("Features", allFeatures));

        var trainer = preprocessingPipeline
            .Append(_mlContext.Regression.Trainers.LightGbm(
                labelColumnName: "AverageDamage",
                featureColumnName: "Features",
                numberOfLeaves: 64,
                minimumExampleCountPerLeaf: 10,
                learningRate: 0.1,
                numberOfIterations: 100));

        var pipeline = preprocessingPipeline.Append(trainer);
        var model = pipeline.Fit(data);

        _mlContext.Model.Save(model, data.Schema, streamToSaveto);
    }
}