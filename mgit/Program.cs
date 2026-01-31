using System.CommandLine;
using System.Threading.Tasks;
using mgit.Config;
using mgit.Options;
using mgit.Runner;

namespace mgit
{
    internal static class Program
    {
        private static AppConfig? AppConfig;

        private static int Main(string[] args)
        {
            var root = new RootCommand("mgit lets you run common git workflows across many repositories from one CLI.");

            var debugOption = new Option<bool>(new[] { "--debug" },
                "Increase console verbosity to Debug.");
            root.AddGlobalOption(debugOption);

            var initCommand = new Command("init", "Initialize the project with default settings.");
            initCommand.SetHandler(
                (bool debug) => RunInit(new InitOptions { Debug = debug }),
                debugOption
            );
            root.AddCommand(initCommand);

            var statusCommand = new Command("status", "Show the status of the repositories.");
            statusCommand.SetHandler(
                (bool debug) => RunStatus(new StatusOptions { Debug = debug }),
                debugOption
            );
            root.AddCommand(statusCommand);

            var logNumberOption = new Option<int>(new[] { "-n", "--number" },
                () => 3,
                "Number of commits to show.");
            var logNumberArgument = new Argument<int?>("number",
                () => null,
                "Number of commits to show (overrides -n/--number).");
            var logCommand = new Command("log", "Show the commit logs of the repositories.");
            logCommand.AddOption(logNumberOption);
            logCommand.AddArgument(logNumberArgument);
            logCommand.SetHandler(
                (int numberOption, int? numberArgument, bool debug) =>
                {
                    var count = numberArgument ?? numberOption;
                    return Task.FromResult(RunLog(new LogOptions { Number = count, Debug = debug }));
                },
                logNumberOption,
                logNumberArgument,
                debugOption
            );
            root.AddCommand(logCommand);

            var pullCommand = new Command("pull", "Pull changes from the remote repositories.");
            pullCommand.SetHandler(
                (bool debug) => RunPull(new PullOptions { Debug = debug }),
                debugOption
            );
            root.AddCommand(pullCommand);

            var checkoutBranchOption = new Option<string>(new[] { "-b", "--branch" },
                "The branch or commit to checkout.")
            {
                IsRequired = true
            };
            var checkoutCommand = new Command("checkout", "Checkout a specific branch or commit.");
            checkoutCommand.AddOption(checkoutBranchOption);
            checkoutCommand.SetHandler(
                (string branch, bool debug) => RunCheckout(new CheckoutOptions { Branch = branch, Debug = debug }),
                checkoutBranchOption,
                debugOption
            );
            root.AddCommand(checkoutCommand);

            var addPathOption = new Option<string?>(new[] { "-p", "--path" },
                "Path to the file or directory to add. Defaults to all files.");
            var addPathArgument = new Argument<string?>("path",
                () => null,
                "Path to the file or directory to add. Defaults to all files.");
            var addCommand = new Command("add", "Add file contents to the index.");
            addCommand.AddOption(addPathOption);
            addCommand.AddArgument(addPathArgument);
            addCommand.SetHandler(
                (string? pathOption, string? pathArgument, bool debug) =>
                    RunAdd(new AddOptions { Path = pathOption ?? pathArgument, Debug = debug }),
                addPathOption,
                addPathArgument,
                debugOption
            );
            root.AddCommand(addCommand);

            var branchCreateOption = new Option<string?>(new[] { "-c", "--create" },
                "Create a new branch with the specified name.");
            var branchDeleteOption = new Option<string?>(new[] { "-d", "--delete" },
                "Delete the branch with the specified name.");
            var branchListOption = new Option<bool>(new[] { "-l", "--list" },
                "List all branches in the repository.");
            var branchCommand = new Command("branch", "Manage branches in the repository.");
            branchCommand.AddOption(branchCreateOption);
            branchCommand.AddOption(branchDeleteOption);
            branchCommand.AddOption(branchListOption);
            branchCommand.SetHandler(
                (string? create, string? delete, bool list, bool debug) =>
                    RunBranch(new BranchOptions
                    {
                        Create = create,
                        Delete = delete,
                        List = list,
                        Debug = debug
                    }),
                branchCreateOption,
                branchDeleteOption,
                branchListOption,
                debugOption
            );
            root.AddCommand(branchCommand);

            var commitMessageOption = new Option<string>(new[] { "-m", "--message" },
                "Commit message.")
            {
                IsRequired = false
            };
            var commitMessageArgument = new Argument<string?>("message",
                "Commit message rendered to the translator.");
            var commitCommand = new Command("commit", "Commit changes to the repository.");
            commitCommand.AddOption(commitMessageOption);
            commitCommand.AddArgument(commitMessageArgument);
            commitCommand.SetHandler(
                (string? messageOption, string? messageArgument, bool debug) =>
                {
                    var message = messageOption ?? messageArgument;
                    if (string.IsNullOrEmpty(message))
                    {
                        throw new ArgumentException("Commit message is required");
                    }

                    return Task.FromResult(RunCommit(new CommitOptions { Message = message, Debug = debug }));
                },
                commitMessageOption,
                commitMessageArgument,
                debugOption
            );
            root.AddCommand(commitCommand);

            var pushCommand = new Command("push", "Push changes to the remote repositories.");
            pushCommand.SetHandler(
                (bool debug) => RunPush(new PushOptions { Debug = debug }),
                debugOption
            );
            root.AddCommand(pushCommand);

            var mergeBranchOption = new Option<string>(new[] { "-b", "--branch" }, "The branch name.")
            {
                IsRequired = true
            };
            var mergeCommand = new Command("merge", "Merge a branch");
            mergeCommand.AddOption(mergeBranchOption);
            mergeCommand.SetHandler(
                (string branch, bool debug) => RunMerge(new MergeOptions { Branch = branch, Debug = debug }),
                mergeBranchOption,
                debugOption
            );
            root.AddCommand(mergeCommand);

            return root.Invoke(args);
        }

        private static int RunMerge(MergeOptions arg)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new MergeRunner(AppConfig, arg);
            return runner.Run();
        }

        private static int RunPull(PullOptions arg)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new PullRunner(AppConfig, arg);
            return runner.Run();
        }

        private static int RunPush(PushOptions options)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new PushRunner(AppConfig, options);
            return runner.Run();
        }


        private static int RunLog(LogOptions options)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new LogRunner(AppConfig, options);
            return runner.Run();
        }

        private static int RunCheckout(CheckoutOptions arg)
        {
            LoadAppConfig();


            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new CheckoutRunner(AppConfig, arg);
            return runner.Run();
        }

        private static int RunAdd(AddOptions options)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new AddRunner(AppConfig, options);
            return runner.Run();
        }

        private static int RunStatus(StatusOptions arg)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new StatusRunner(AppConfig, arg);
            return runner.Run();
        }

        private static int RunCommit(CommitOptions options)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new CommitRunner(AppConfig, options);
            return runner.Run();
        }


        private static int RunBranch(BranchOptions options)
        {
            LoadAppConfig();

            if (AppConfig == null)
            {
                throw new InvalidOperationException("AppConfig is not loaded");
            }

            var runner = new BranchRunner(AppConfig, options);
            return runner.Run();
        }

        private static int RunInit(InitOptions arg)
        {
            AppConfig = new AppConfig();
            var runner = new InitRunner(AppConfig, arg);
            return runner.Run();
        }

        private static void LoadAppConfig()
        {
            var path = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(path, AppConfig.YmlFile);
            if (!File.Exists(configPath)) throw new FileNotFoundException("Config file not found", configPath);
            var yaml = File.ReadAllText(configPath);
            AppConfig = AppConfig.FromYml(yaml);
        }
    }
}
