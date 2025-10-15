using CommandLine;

namespace mgit.Options;

[Verb("commit", HelpText = "Commit changes to the repository.")]
public class CommitOptions
{
    [Option('m', "message", Required = true, HelpText = "Commit message.")]
    public string Message { get; set; }
}