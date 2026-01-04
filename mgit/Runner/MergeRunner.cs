using LibGit2Sharp;
using mgit.Config;
using MergeOptions = mgit.Options.MergeOptions;

namespace mgit.Runner;

public class MergeRunner : Runner
{
    private readonly MergeOptions _options;

    public MergeRunner(AppConfig appConfig, MergeOptions options) : base(appConfig)
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
                var branch = repo.Branches[_options.Branch];
                if (branch == null)
                {
                    Console.WriteLine($"  Branch '{_options.Branch}' not found in repository '{repoPath}'.");
                    continue;
                }

                var signature = new Signature(AppConfig.Author.Name, AppConfig.Author.Email, DateTimeOffset.Now);
                var result = repo.Merge(branch, signature);
                if (result.Status == MergeStatus.Conflicts)
                {
                    Console.WriteLine($"  Merge conflicts occurred in repository '{repoPath}'.");
                }
                else
                {
                    Console.WriteLine(
                        $"  Merged branch '{_options.Branch}' into current branch in repository '{repoPath}'.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error merging branch in {repoPath}: {ex.Message}");
            }
        }

        return 0;
    }
}