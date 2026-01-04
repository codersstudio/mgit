using System.Diagnostics;
using mgit.Config;

namespace mgit.Runner;

public abstract class Runner
{
    protected readonly AppConfig AppConfig;

    protected Runner(AppConfig appConfig)
    {
        AppConfig = appConfig;
    }

    public abstract int Run();

    protected static (string username, string password)? GetGitCredentials(string url)
    {
        var psi = new ProcessStartInfo
        {
            FileName = "git",
            Arguments = "credential fill",
            RedirectStandardInput = true,
            RedirectStandardOutput = true,
            UseShellExecute = false
        };

        using var process = Process.Start(psi);
        if (process == null)
            return null;

        // Git credential-helper 프로토콜에 맞게 요청
        process.StandardInput.WriteLine($"url={url}");
        process.StandardInput.WriteLine();
        process.StandardInput.Flush();

        string output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();

        string? user = null, pass = null;
        foreach (var line in output.Split('\n'))
        {
            if (line.StartsWith("username="))
                user = line.Substring("username=".Length).Trim();
            if (line.StartsWith("password="))
                pass = line.Substring("password=".Length).Trim();
        }

        if (user != null && pass != null)
            return (user, pass);

        return null;
    }
}