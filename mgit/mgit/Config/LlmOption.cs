using System.Text;
using mgit.Util;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace mgit.Config;

public class LlmOption
{
    [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string Provider { get; set; } = LlmProvider.Ollama;

    [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string Model { get; set; } = "gpt-oss:20b";


    [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string Url { get; set; } = "http://localhost:11434";


    [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
    public string ApiKey { get; set; } = "";

    public static LlmOption FromYml(string yml)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        return deserializer.Deserialize<LlmOption>(yml);
    }

    public static void SaveYml(string filePath, LlmOption config)
    {
        // camel case , double quote value
        var serializer = new SerializerBuilder()
            .ConfigureDefaultValuesHandling(DefaultValuesHandling.OmitNull)
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();

        var yml = serializer.Serialize(config);
        FileUtil.WriteAllText(filePath, yml);
    }

    public void ToComment(StringBuilder builder, int indent)
    {
        var prefix1 = new string(' ', indent * 2) + "  ";
        var prefix2 = new string(' ', indent * 2) + "  ";

        // 프로바이더를 설정합니다.
        builder.Append("# ").Append(prefix1)
            .AppendLine(
                $"provider: \"{Provider}\" # The LLM provider to use. For example, '{LlmProvider.Ollama}' '{LlmProvider.Gemini}' '{LlmProvider.Chatgpt}'");
        // 모델명을 설정합니다.
        builder.Append("# ").Append(prefix1).AppendLine($"model: \"{Model}\" # The model to use for LLM requests.");
        // 호스트 URL을 설정합니다.
        builder.Append("# ").Append(prefix2).AppendLine($"url: \"{Url}\" # The host URL for the LLM service.");
        // API 키를 설정합니다.
        builder.Append("# ").Append(prefix2)
            .AppendLine($"apiKey: \"{ApiKey}\" # The API key for authenticating with the LLM service.");
    }
}