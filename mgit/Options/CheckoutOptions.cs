using CommandLine;

namespace mgit.Options;

[Verb("checkout", HelpText = "Checkout a specific branch or commit.")]
public class CheckoutOptions : BaseOptions
{
    [Option('b', "branch", Required = true, HelpText = "The branch or commit to checkout.")]
    public string Branch { get; set; }
}