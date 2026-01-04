using CommandLine;

namespace mgit.Options;

[Verb("add", HelpText = "Add file contents to the index.")]
public class AddOptions : BaseOptions
{
    [Option('p', "path", Required = false, HelpText = "Path to the file or directory to add. Defaults to all files.")]
    public string? Path { get; set; }
}