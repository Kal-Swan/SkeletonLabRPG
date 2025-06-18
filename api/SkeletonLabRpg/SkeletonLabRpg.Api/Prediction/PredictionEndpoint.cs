using Microsoft.AspNetCore.Mvc;
using SkeletonLabRpg.Api.Endpoints;
using SkeletonLabRpg.Api.Prediction.Constants;
using SkeletonLabRpg.Api.Prediction.Models;
using SkeletonLabRpg.Common.Constants;
using SkeletonLabRpg.Common.Database;
using SkeletonLabRpg.Common.Database.Models;
using SkeletonLabRpg.Common.Extensions;
using SkeletonLabRpg.Ml.Extensions;
using SkeletonLabRpg.Ml.Services.Interfaces;

namespace SkeletonLabRpg.Api.Prediction;

public static class PredictionEndpoint
{
    public class Endpoint : IEndpoint
    {
        public void MapEndpoint(IEndpointRouteBuilder builder)
        {
            builder.MapPost(PredictionConstants.Base, Handle);
        }

        private static async Task Handle(
            [FromBody] PredictionRequest request,
            [FromServices] IDamagePrediction damagePrediction,
            [FromServices] IRepository<CharacterAttributes> repository)
        {
            var classes = ClassTags.ClassTagsMap
                .Where(pair => request.CharacterTags.Any(tag => pair.Value.Contains(tag)))
                .Select(pair => pair.Key);
            // get all character builds based on tags from csv in blob or cosmos
            var characterAttributes = await repository.GetMany(attributes => 
                attributes.AccountEmail == "" && classes.Contains(attributes.ClassType));

            // optional: choose the evenly matched builds
            
            // convert them to ml model
            var characterMlModels = characterAttributes
                .Select(attributes => attributes.AssignCharacterTagsForAi().ConvertToAttributeTrainingModel());
            // pass them into the prediction engine
            var prediction = damagePrediction.PredictDamage(characterMlModels);
        }
    }
}