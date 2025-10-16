using CommandLine;
using LibGit2Sharp;
using mgit.Config;
using mgit.Llm.Client;
using mgit.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using CommitOptions = mgit.Options.CommitOptions;
using StatusOptions = mgit.Options.StatusOptions;
using AddOptions = mgit.Options.AddOptions;
using CheckoutOptions = mgit.Options.CheckoutOptions;


namespace mgit
{
    internal static class Program
    {
        private static AppConfig? _appConfig;

        private static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<InitOptions, StatusOptions, LogOptions, CheckoutOptions, AddOptions, BranchOptions,
                    CommitOptions>(args)
                .MapResult<InitOptions, StatusOptions, LogOptions, CheckoutOptions, AddOptions, BranchOptions,
                    CommitOptions, int>(
                    RunInit,
                    RunStatus,
                    RunLog,
                    RunCheckout,
                    RunAdd,
                    RunBranch,
                    RunCommit,
                    errs => 0
                );
        }

        private static int RunLog(LogOptions arg)
        {
            LoadAppConfig();

            if (_appConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            foreach (var repoPath in _appConfig.Repos)
            {
                try
                {
                    using var repo = new Repository(repoPath);
                    Console.WriteLine($"Repository: {repoPath}");
                    foreach (var commit in repo.Commits.Take(arg.Number))
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

        private static int RunCheckout(CheckoutOptions arg)
        {
            LoadAppConfig();

            if (_appConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            if (string.IsNullOrEmpty(arg.Branch))
            {
                throw new ArgumentException("Branch name is required", nameof(arg.Branch));
            }

            foreach (var repoPath in _appConfig.Repos)
            {
                try
                {
                    using var repo = new Repository(repoPath);
                    var branch = repo.Branches[arg.Branch];
                    if (branch == null)
                    {
                        Console.WriteLine($"  Branch '{arg.Branch}' not found in repository '{repoPath}'.");
                        continue;
                    }

                    Commands.Checkout(repo, branch);
                    Console.WriteLine($"  Checked out branch '{arg.Branch}' in repository '{repoPath}'.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error checking out branch in {repoPath}: {ex.Message}");
                }
            }

            return 0;
        }

        private static int RunAdd(AddOptions arg)
        {
            LoadAppConfig();

            if (_appConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            foreach (var repoPath in _appConfig.Repos)
            {
                try
                {
                    using var repo = new Repository(repoPath);
                    Commands.Stage(repo, arg.Path ?? "*");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Error adding files in {repoPath}: {ex.Message}");
                }
            }


            return 0;
        }

        private static int RunStatus(StatusOptions arg)
        {
            LoadAppConfig();

            if (_appConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            foreach (var repoPath in _appConfig.Repos)
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

        private static int RunCommit(CommitOptions arg)
        {
            LoadAppConfig();

            if (_appConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            if (string.IsNullOrEmpty(arg.Message))
            {
                throw new ArgumentException("Commit message is required", nameof(arg.Message));
            }

            var message = arg.Message;

            var llmClient = LlmClient.GetLlmClient(_appConfig.Llm);
            if (llmClient == null)
            {
                throw new InvalidOperationException("Failed to create LLM client");
            }

            // 커밋 메세지 영어로 번역. 번역 문장만 반환
            var prompt = "Translate the following commit message to English. Only provide the translated message without any additional text:\n\"\"\"\n" + message + "\n\"\"\"";

            // var prompt = "Translate the following commit message to English:\n\"\"\"\n" + message + "\n\"\"\"";
            var translatedMessage = llmClient.GetCompletion(prompt).Result.Trim();
            
            Console.WriteLine("  " + message);
            Console.WriteLine("  -> " + translatedMessage);

            foreach (var repoPath in _appConfig.Repos)
            {
                try
                {
                    using var repo = new Repository(repoPath);
                    Commands.Stage(repo, "*");
                    var author = new Signature("mgit", _appConfig.Author.Email, DateTimeOffset.Now);
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


        private static int RunBranch(BranchOptions arg)
        {
            LoadAppConfig();

            if (_appConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            if (string.IsNullOrEmpty(arg.Create) == false)
            {
                BranchCreate(_appConfig, arg.Create);
            }
            else if (string.IsNullOrEmpty(arg.Delete) == false)
            {
                BranchDelete(_appConfig, arg.Delete);
            }
            else if (arg.List)
            {
                BranchList(_appConfig);
            }
            else
            {
                BranchList(_appConfig);
            }

            return 0;
        }

        private static void BranchList(AppConfig appConfig)
        {
            foreach (var repoPath in appConfig.Repos)
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

        private static void BranchDelete(AppConfig appConfig, string argDelete)
        {
            foreach (var repoPath in appConfig.Repos)
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

        private static void BranchCreate(AppConfig appConfig, string argCreate)
        {
            foreach (var repoPath in appConfig.Repos)
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

        private static int RunInit(InitOptions arg)
        {
            _appConfig = new AppConfig();

            var rootDir = Directory.GetCurrentDirectory();

            foreach (var dir in Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories))
            {
                var gitDir = Path.Combine(dir, ".git");
                if (Directory.Exists(gitDir))
                {
                    _appConfig.Repos.Add(dir);

                    // try
                    // {
                    //     using var repo = new Repository(dir);
                    //     var remote = repo.Network.Remotes["origin"];
                    //     if (remote != null)
                    //     {
                    //         repos.Add(new RepoInfo { Path = dir, Url = remote.Url });
                    //     }
                    // }
                    // catch
                    // {
                    //     /* 무시 */
                    // }
                }
            }

            var serializer = new SerializerBuilder()
                .WithNamingConvention(CamelCaseNamingConvention.Instance)
                .Build();

            var yaml = serializer.Serialize(_appConfig);
            File.WriteAllText(AppConfig.YmlFile, yaml);

            return 0;
        }

        private static void LoadAppConfig()
        {
            var path = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(path, AppConfig.YmlFile);
            if (!File.Exists(configPath)) throw new FileNotFoundException("Config file not found", configPath);
            var yaml = File.ReadAllText(configPath);
            _appConfig = AppConfig.FromYml(yaml);
        }
    }
}