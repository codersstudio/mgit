using LibGit2Sharp;
using mgit.Config;
using CheckoutOptions = mgit.Options.CheckoutOptions;

namespace mgit.Runner;

public class CheckoutRunner : Runner
{
    private CheckoutOptions _options;

    public CheckoutRunner(AppConfig appConfig, CheckoutOptions options) : base(appConfig)
    {
        _options = options;
    }

    public override int Run()
    {
        if (string.IsNullOrEmpty(_options.Branch))
        {
            throw new ArgumentException("Branch name is required", nameof(_options.Branch));
        }

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

                Commands.Checkout(repo, branch);
                Console.WriteLine($"  Checked out branch '{_options.Branch}' in repository '{repoPath}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error checking out branch in {repoPath}: {ex.Message}");
            }
        }

        return 0;
    }
}