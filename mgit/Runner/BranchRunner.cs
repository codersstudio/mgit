using LibGit2Sharp;
using mgit.Config;
using mgit.Options;

namespace mgit.Runner;

public class BranchRunner : Runner
{
    private BranchOptions _options;

    public BranchRunner(AppConfig appConfig, BranchOptions options) : base(appConfig)
    {
        _options = options;
    }

    public override int Run()
    {
        if (string.IsNullOrEmpty(_options.Create) == false)
        {
            BranchCreate(_options.Create);
        }
        else if (string.IsNullOrEmpty(_options.Delete) == false)
        {
            BranchDelete(_options.Delete);
        }
        else if (_options.List)
        {
            BranchList();
        }
        else
        {
            BranchList();
        }

        return 0;
    }

    private void BranchList()
    {
        foreach (var repoPath in AppConfig.Repos)
        {
            try
            {
                using var repo = new Repository(repoPath);
                Console.WriteLine($"Repository: {repoPath}");
                foreach (var branch in repo.Branches)
                {
                    var currentMarker = branch.IsCurrentRepositoryHead ? "*" : " ";
                    Console.WriteLine($"  {currentMarker} {branch.FriendlyName}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error accessing repository at {repoPath}: {ex.Message}");
            }
        }
    }

    private void BranchDelete(string argDelete)
    {
        foreach (var repoPath in AppConfig.Repos)
        {
            try
            {
                using var repo = new Repository(repoPath);
                var branch = repo.Branches[argDelete];
                if (branch == null)
                {
                    Console.WriteLine($"  Branch '{argDelete}' not found in repository '{repoPath}'.");
                    continue;
                }

                if (branch.IsCurrentRepositoryHead)
                {
                    Console.WriteLine(
                        $"  Cannot delete the current checked-out branch '{argDelete}' in repository '{repoPath}'.");
                    continue;
                }

                repo.Branches.Remove(branch);
                Console.WriteLine($"  Deleted branch '{argDelete}' in repository '{repoPath}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error deleting branch in {repoPath}: {ex.Message}");
            }
        }
    }

    private void BranchCreate(string argCreate)
    {
        foreach (var repoPath in AppConfig.Repos)
        {
            try
            {
                using var repo = new Repository(repoPath);
                var existingBranch = repo.Branches[argCreate];
                if (existingBranch != null)
                {
                    Console.WriteLine($"  Branch '{argCreate}' already exists in repository '{repoPath}'.");
                    continue;
                }

                var newBranch = repo.CreateBranch(argCreate);
                Console.WriteLine($"  Created branch '{argCreate}' in repository '{repoPath}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error creating branch in {repoPath}: {ex.Message}");
            }
        }
    }
}