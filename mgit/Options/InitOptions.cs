using CommandLine;

namespace mgit.Options;

[Verb("init", HelpText = "Initialize the project with default settings.")]
public class InitOptions : BaseOptions
{
}