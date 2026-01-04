using mgit.Config;
using mgit.Options;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace mgit.Runner;

public class InitRunner : Runner
{
    private InitOptions _options;

    public InitRunner(AppConfig appConfig, InitOptions options) : base(appConfig)
    {
        _options = options;
    }

    public override int Run()
    {
        var rootDir = Directory.GetCurrentDirectory();

        foreach (var dir in Directory.GetDirectories(rootDir, "*", SearchOption.AllDirectories))
        {
            var gitDir = Path.Combine(dir, ".git");
            if (Directory.Exists(gitDir))
            {
                AppConfig.Repos.Add(dir);
            }
        }

        var serializer = new SerializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yaml = serializer.Serialize(AppConfig);
        File.WriteAllText(AppConfig.YmlFile, yaml);

        return 0;
    }
}