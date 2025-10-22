using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace mgit.Config;

public class AppConfig
{
    public const string YmlFile = "mgit.yml";

    [YamlMember(Order = 1)] public LlmOption Llm { get; set; } = new();

    [YamlMember(Order = 2)]
    public Author Author { get; set; } = new();

    [YamlMember(Order = 3)]
    public List<string> Repos { get; set; } = [];

    public static AppConfig FromYml(string yaml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance) // llm -> Llm 매핑
            .IgnoreUnmatchedProperties() // 여분 키 무시 (선택)
            .Build();
        return deserializer.Deserialize<AppConfig>(yaml);
    }
}