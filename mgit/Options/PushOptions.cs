using CommandLine;

namespace mgit.Options;

[Verb("push", HelpText = "Push changes to the remote repositories.")]
public class PushOptions : BaseOptions
{
}