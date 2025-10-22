using mgit.Config;
using OpenAI.Chat;

namespace mgit.Llm.Client;

public class ChatgptClient : LlmClient
{
    private readonly ChatClient _apiClient;

    public ChatgptClient(LlmOption llmOptions) : base(llmOptions)
    {
        // var uri = new Uri(llmOptions.Url);
        _apiClient = new ChatClient(model: llmOptions.Model,
            apiKey: Environment.GetEnvironmentVariable(llmOptions.ApiKey));

        // var client = new ChatClient(model: "gpt-4o", apiKey: Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
    }

    public override Task<string> GetCompletion(string source)
    {
        ChatCompletion completion = _apiClient.CompleteChat([source],
            new ChatCompletionOptions
            {
                Temperature = 0f, // 온도 설정
                TopP = 0f // Top P 설정
            });
        return Task.FromResult(completion.Content[0].Text);
    }
}