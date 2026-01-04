using LibGit2Sharp;
using mgit.Config;
using mgit.Options;

namespace mgit.Runner;

public class AddRunner : Runner
{
    private AddOptions _options;

    public AddRunner(AppConfig appConfig, AddOptions options) : base(appConfig)
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
                Commands.Stage(repo, _options.Path ?? "*");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error adding files in {repoPath}: {ex.Message}");
            }
        }


        return 0;
    }
}