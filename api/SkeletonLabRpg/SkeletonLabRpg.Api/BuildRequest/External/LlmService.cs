using SkeletonLabRpg.Api.BuildRequest.Models;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.BuildRequest.External;

public class LlmService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<LlmService> _logger;

    public LlmService(HttpClient httpClient, ILogger<LlmService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<Models.LlmBuilds> GetBuildsByQuestion(string question, string rpgSystem)
    {
        _logger.LogInformation("LlmService, GetBuildsByQuestion arguments: Question: {QUESTION}, RPG System: {RPGSYSTEM}  ", question, rpgSystem);
        var requestBody = new RpgQuestionRequest { Question = question, RpgSystem = rpgSystem };
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("question", requestBody);
        response.EnsureSuccessStatusCode();
        var characterBuild = await response.Content.ReadFromJsonAsync<Models.LlmBuilds>();

        if (characterBuild is null)
        {
            throw new InvalidApiResponseException($"Failed to retrieve character builds from API in class {nameof(LlmService)} and method {nameof(GetBuildsByQuestion)} with question {question} and rpgSystem {rpgSystem}");
        }

        return characterBuild;
    }
}