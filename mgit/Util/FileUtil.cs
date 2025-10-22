using System.Text;

namespace mgit.Util;

public static class FileUtil
{
    public static void WriteAllText(string path, string str)
    {
        File.WriteAllText(path, str, new UTF8Encoding(false));
    }
}