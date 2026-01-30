using System.Text;
using mgit.Config;
using OllamaSharp;
using OllamaSharp.Models;

namespace mgit.Llm.Client;

public class OllamaClient : LlmClient
{
    private readonly OllamaApiClient _apiClient;

    public OllamaClient(LlmOption llmOptions) : base(llmOptions)
    {
        var uri = new Uri(llmOptions.Url);
        _apiClient = new OllamaApiClient(uri);
        _apiClient.SelectedModel = llmOptions.Model;
    }

    public override async Task<string> GetCompletion(string source)
    {
        try
        {
            var sb = new StringBuilder();
            var request = new GenerateRequest
            {
                Model = _apiClient.SelectedModel,
                Prompt = source,
                Stream = false,
                Options = new RequestOptions
                {
                    NumThread = Math.Max(1, Environment.ProcessorCount),
                    NumBatch = 512,
                    F16kv = true,
                    UseMmap = true
                }
            };

            var res = _apiClient.GenerateAsync(request);

            await foreach (var stream in res)
            {
                sb.Append(stream?.Response);
            }

            return sb.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }

        return string.Empty;

        // var enumerator = apiClient.GenerateAsync(source)
        //     .ConfigureAwait(false)
        //     .GetAsyncEnumerator();
        //
        // try
        // {
        //     // MoveNextAsync().AsTask()로 Task<IAsyncEnumerator>를 얻어 동기 블로킹
        //     while (enumerator.MoveNextAsync().GetAwaiter().GetResult())
        //     {
        //         var stream = enumerator.Current;
        //         if (stream?.Response is not null)
        //             sb.Append(stream.Response);
        //     }
        // }
        // finally
        // {
        //     // DisposeAsync도 마찬가지로 블로킹 호출
        //     enumerator.DisposeAsync().GetAwaiter().GetResult();
        // }
        //
        // return sb.ToString();
    }

    // public string GetCompletion(string source)
    // {
    //     var sb = new StringBuilder();
    //     await foreach (var stream in apiClient.GenerateAsync(source))
    //     {
    //         sb.Append(stream?.Response);
    //     }
    //
    //     return sb.ToString();
    // }
}
