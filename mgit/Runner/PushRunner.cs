using LibGit2Sharp;
using mgit.Config;
using PushOptions = mgit.Options.PushOptions;

namespace mgit.Runner;

public class PushRunner : Runner
{
    private PushOptions _options;

    public PushRunner(AppConfig appConfig, PushOptions options) : base(appConfig)
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

                var options = new LibGit2Sharp.PushOptions
                {
                    CredentialsProvider = (url, user, cred) =>
                        new UsernamePasswordCredentials
                        {
                            Username = creds.Value.username,
                            Password = creds.Value.password
                        }
                };

                repo.Network.Push(remote, @"refs/heads/" + repo.Head.FriendlyName, options);
                Console.WriteLine($"  Pushed to remote 'origin' in repository '{repoPath}'.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Error pushing in {repoPath}: {ex.Message}");
            }
        }

        return 0;
    }
}