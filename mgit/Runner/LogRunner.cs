using LibGit2Sharp;
using mgit.Config;
using mgit.Options;

namespace mgit.Runner;

public class LogRunner : Runner
{
    private LogOptions _options;

    public LogRunner(AppConfig appConfig, LogOptions options) : base(appConfig)
    {
        _options = options;
    }

    public override int Run()
    {
        foreach (var repoPath in AppConfig.Repos)
        {
            try
            {
                using var repo = new Repository(repoPath);
                Console.WriteLine($"Repository: {repoPath}");
                foreach (var commit in repo.Commits.Take(_options.Number))
                {
                    Console.WriteLine($"  Commit: {commit.Sha}");
                    Console.WriteLine($"  Author: {commit.Author.Name} <{commit.Author.Email}>");
                    Console.WriteLine($"  Date:   {commit.Author.When}");
                    Console.WriteLine();
                    Console.WriteLine($"      {commit.MessageShort}");
                    Console.WriteLine();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error accessing repository at {repoPath}: {ex.Message}");
            }
        }

        return 0;
    }
}