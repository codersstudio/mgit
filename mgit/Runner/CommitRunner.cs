using System.Text;
using LibGit2Sharp;
using mgit.Config;
using mgit.Llm.Client;
using Scriban;
using CommitOptions = mgit.Options.CommitOptions;

namespace mgit.Runner;

public class CommitRunner : Runner
{
    private CommitOptions _options;

    public CommitRunner(AppConfig appConfig, CommitOptions options) : base(appConfig)
    {
        _options = options;
    }

    public override int Run()
    {
        if (string.IsNullOrEmpty(_options.Message))
        {
            throw new ArgumentException("Commit message is required", nameof(_options.Message));
        }

        var message = _options.Message;

        var llmClient = LlmClient.GetLlmClient(AppConfig.Llm);
        if (llmClient == null)
        {
            throw new InvalidOperationException("Failed to create LLM client");
        }


        var systemMd = GetPrompt();
        var systemTemplate = Template.Parse(systemMd);
        var prompt = systemTemplate.Render(new { InputText = message });

        // 커밋 메세지 영어로 번역. 번역 문장만 반환
        // var prompt =
        //     "Translate the following commit message to English. Only provide the translated message without any additional text:\n\"\"\"\n" +
        //     message + "\n\"\"\"";
        var jsonResponse = llmClient.GetCompletion(prompt).Result.Trim();

        if (_options.Debug)
        {
            Console.WriteLine(jsonResponse);
        }

        var response = CommitFormatResponse.Parse(jsonResponse);

        var breaking = "";
        if (response.Breaking)
        {
            breaking = "!";
        }

        var translatedMessage = $"{response.Type}({response.Scope}){breaking}: {response.Subject}";

        Console.WriteLine($"  {message} -> {translatedMessage}");

        foreach (var repoPath in AppConfig.Repos)
        {
            try
            {
                using var repo = new Repository(repoPath);
                Commands.Stage(repo, "*");
                var author = new Signature(AppConfig.Author.Name, AppConfig.Author.Email, DateTimeOffset.Now);
                var committer = author;
                repo.Commit(translatedMessage, author, committer);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error committing in {repoPath}: {ex.Message}");
            }
        }


        return 0;
    }

    private string GetPrompt()
    {
        var asm = typeof(CommitRunner).Assembly;
        const string version = "v1";
        const string language = "en";
        const string role = "system";
        const string resourceName = $"mgit.Resources.Prompts.{version}.{language}.{role}.md";

        using var stream = asm.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            throw new FileNotFoundException($"Prompt template not found: {resourceName}");
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        var value = reader.ReadToEnd();
        return value;
    }
}