using CommandLine;

namespace mgit.Options;

public class BaseOptions
{
    // debug 옵션: 예) --debug
    [Option('d', "debug", HelpText = "Increase console verbosity to Debug.")]
    public bool Debug { get; set; }
}