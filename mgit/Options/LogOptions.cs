using CommandLine;

namespace mgit.Options;

[Verb("log", HelpText = "Show the commit logs of the repositories.")]
public class LogOptions
{
    [Option('n', "number", Required = false, HelpText = "Number of commits to show.", Default = 3)]
    public int Number { get; set; } = 3;
}