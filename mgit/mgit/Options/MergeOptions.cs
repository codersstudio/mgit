using CommandLine;

namespace mgit.Options;

[Verb("merge", HelpText = "Merge a branch")]
public class MergeOptions
{
    [Option('b', "branch", Required = true, HelpText = "The branch name.")]
    public string Branch { get; set; }
}