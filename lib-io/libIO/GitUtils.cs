
using FileLib;
namespace libIO;

public class GitUtils
{
    static HashSet<string> GetIgnoredFiles(string gitIgnorePath, string rootPath)
    {
        HashSet<string> ignoredFiles = new HashSet<string>();

        if (!FileUtils.FileExists(gitIgnorePath))
        {
            return ignoredFiles;
        }
        
        string[] ignorePatterns = File.ReadAllLines(gitIgnorePath)
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                .ToArray();

        foreach (string pattern in ignorePatterns)
        {
            string fullPath = Path.Combine(rootPath, pattern.Trim());
            if (Directory.Exists(fullPath))
            {
                string[] files = Directory.GetFiles(fullPath, "*", SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    ignoredFiles.Add(Path.GetFullPath(file));
                }
            }
            else
            {
                ignoredFiles.Add(Path.GetFullPath(fullPath));
            }
        }

        return ignoredFiles;
    }
}