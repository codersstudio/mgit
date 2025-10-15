using DotnetGeminiSDK.Config;
using DotnetGeminiSDK.Model.Request;
using mgit.Config;

namespace mgit.Llm.Client;

public class GeminiClient : LlmClient
{
    private readonly DotnetGeminiSDK.Client.GeminiClient _geminiClient;

    public GeminiClient(LlmOption llmOptions) : base(llmOptions)
    {
        var config = new GoogleGeminiConfig
        {
            ApiKey = Environment.GetEnvironmentVariable(llmOptions.ApiKey) ?? string.Empty,
            TextBaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/" + llmOptions.Model,
        };
        _geminiClient = new DotnetGeminiSDK.Client.GeminiClient(config);
    }

    public override async Task<string> GetCompletion(string prompt)
    {
        var config = new GenerationConfig
        {
            Temperature = 0,
            TopP = 0,
            MaxOutputTokens = 10000,
        };

        var res = await _geminiClient.TextPrompt(prompt, config);

        if (res == null)
        {
            return string.Empty;
        }

        return res.Candidates[0].Content.Parts[0].Text;
    }
}