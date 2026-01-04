using LibGit2Sharp;
using mgit.Config;
using StatusOptions = mgit.Options.StatusOptions;

namespace mgit.Runner;

public class StatusRunner : Runner
{
    private StatusOptions _options;

    public StatusRunner(AppConfig appConfig, StatusOptions options) : base(appConfig)
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
                var status = repo.RetrieveStatus();

                Console.WriteLine($"Repository: {repoPath}");
                foreach (var item in status)
                {
                    if (item.State == FileStatus.Ignored)
                    {
                        continue;
                    }

                    Console.WriteLine($"  {item.FilePath} - {item.State}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error accessing repository at {repoPath}: {ex.Message}");
            }
        }

        return 0;
    }
}