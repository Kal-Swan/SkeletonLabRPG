using System.Text.Json;
using SkeletonLabRpg.Api.Llm.Models;
using SkeletonLabRpg.Common.Exceptions;

namespace SkeletonLabRpg.Api.Llm.External;

public class LlmService
{
    private readonly HttpClient _httpClient;
    
    public LlmService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<BuildResponse> GetBuildsByQuestion(string question, string rpgSystem)
    {
        var requestBody = new RpgQuestionRequest { Question = question, RpgSystem = rpgSystem };
        HttpResponseMessage response = await _httpClient.PostAsJsonAsync("question", requestBody);
        response.EnsureSuccessStatusCode();
        var characterBuild = await response.Content.ReadFromJsonAsync<BuildResponse>();

        if (characterBuild is null)
        {
            throw new InvalidApiResponseException($"Failed to retrieve character builds from API in class {nameof(LlmService)} and method {nameof(GetBuildsByQuestion)} with question {question} and rpgSystem {rpgSystem}");
        }

        return characterBuild;
    }
}