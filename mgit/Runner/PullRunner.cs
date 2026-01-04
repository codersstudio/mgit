using LibGit2Sharp;
using mgit.Config;
using PullOptions = mgit.Options.PullOptions;

namespace mgit.Runner;

public class PullRunner : Runner
{
    private PullOptions _options;

    public PullRunner(AppConfig appConfig, PullOptions options) : base(appConfig)
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
                var remote = repo.Network.Remotes["origin"];
                if (remote == null)
                {
                    Console.WriteLine($"  No remote 'origin' found in repository '{repoPath}'.");
                    continue;
                }

                var creds = GetGitCredentials(remote.Url);
                if (creds == null)
                {
                    Console.WriteLine("  No cached credentials found.");
                    continue;
                }

                var options = new LibGit2Sharp.PullOptions
                {
                    FetchOptions = new FetchOptions
                    {
                        CredentialsProvider = (url, user, cred) =>
                            new UsernamePasswordCredentials
                            {
                                Username = creds.Value.username,
                                Password = creds.Value.password
                            }
                    }
                };

                var signature = new Signature(AppConfig.Author.Name, AppConfig.Author.Email, DateTimeOffset.Now);
                Commands.Pull(repo, signature, options);
                Console.WriteLine($"  Pulled from remote 'origin' in repository '{repoPath}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error pulling in {repoPath}: {ex.Message}");
            }
        }

        return 0;
    }
}