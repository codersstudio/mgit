using CommandLine;

namespace mgit.Options;

[Verb("pull", HelpText = "Pull changes from the remote repositories.")]
public class PullOptions : BaseOptions
{
}