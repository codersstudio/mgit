using mgit.Config;

namespace mgit.Llm.Client;

public abstract class LlmClient
{
    protected LlmOption LlmOptions;

    protected LlmClient(LlmOption llmOptions)
    {
        LlmOptions = llmOptions;
    }


    public static LlmClient? GetLlmClient(LlmOption llmOptions)
    {
        return llmOptions.Provider switch
        {
            LlmProvider.Ollama => new OllamaClient(llmOptions),
            LlmProvider.Gemini => new GeminiClient(llmOptions),
            LlmProvider.Chatgpt => new ChatgptClient(llmOptions),
            _ => null
        };
    }

    public abstract Task<string> GetCompletion(string prompt);
}