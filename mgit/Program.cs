using System.Diagnostics;
using CommandLine;
using LibGit2Sharp;
using mgit.Config;
using mgit.Llm.Client;
using mgit.Options;
using mgit.Runner;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using CommitOptions = mgit.Options.CommitOptions;
using StatusOptions = mgit.Options.StatusOptions;
using AddOptions = mgit.Options.AddOptions;
using CheckoutOptions = mgit.Options.CheckoutOptions;
using MergeOptions = mgit.Options.MergeOptions;
using PullOptions = mgit.Options.PullOptions;
using PushOptions = mgit.Options.PushOptions;


namespace mgit
{
    internal static class Program
    {
        private static AppConfig? AppConfig;

        private static void Main(string[] args)
        {
            Parser.Default
                .ParseArguments<InitOptions, StatusOptions, LogOptions, PullOptions, CheckoutOptions, AddOptions,
                    BranchOptions,
                    CommitOptions, PushOptions, MergeOptions>(args)
                .MapResult<InitOptions, StatusOptions, LogOptions, PullOptions, CheckoutOptions, AddOptions,
                    BranchOptions,
                    CommitOptions, PushOptions, MergeOptions, int>(
                    RunInit,
                    RunStatus,
                    RunLog,
                    RunPull,
                    RunCheckout,
                    RunAdd,
                    RunBranch,
                    RunCommit,
                    RunPush,
                    RunMerge,
                    errs => 0
                );
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