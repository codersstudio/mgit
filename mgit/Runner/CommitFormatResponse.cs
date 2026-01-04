using System.Text.Json.Serialization;

namespace mgit.Runner;

public sealed class CommitFormatResponse
{
    [JsonPropertyName("type")] public string Type { get; init; } = null!;

    [JsonPropertyName("scope")] public string? Scope { get; init; }

    [JsonPropertyName("breaking")] public bool Breaking { get; init; }

    [JsonPropertyName("subject")] public string Subject { get; init; } = null!;

    [JsonPropertyName("issues")] public List<string> Issues { get; init; } = [];

    [JsonPropertyName("confidence")] public double Confidence { get; init; }

    [JsonPropertyName("reason")] public string Reason { get; init; } = null!;

    public static CommitFormatResponse Parse(string translatedMessage)
    {
        return System.Text.Json.JsonSerializer.Deserialize<CommitFormatResponse>(translatedMessage)
               ?? throw new InvalidOperationException("Failed to parse CommitFormatResponse");
    }
}