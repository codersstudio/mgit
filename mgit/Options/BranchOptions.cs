using CommandLine;

namespace mgit.Options;

[Verb("branch", HelpText = "Manage branches in the repository.")]
public class BranchOptions
{
    [Option('c', "create", HelpText = "Create a new branch with the specified name.")]
    public string? Create { get; set; }

    [Option('d', "delete", HelpText = "Delete the branch with the specified name.")]
    public string? Delete { get; set; }

    [Option('l', "list", HelpText = "List all branches in the repository.")]
    public bool List { get; set; }
}