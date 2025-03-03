
using FileLib;
using UtilityIO;
namespace libIO;

public class GitUtils
{
    public const string GitIgnoreFileName = ".gitignore";
    static HashSet<string> GetIgnoredFiles(string gitIgnorePath, string rootPath)
    {
        HashSet<string> ignoredFiles = new HashSet<string>();

        if (!FileUtils.FileExists(gitIgnorePath))
        {
            return ignoredFiles;
        }
        
        // Read all lines from the .gitignore file ignoring comments
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
    
    public static List<string> GetNonIgnoredFiles(string rootPath, string searchPattern)
    {
        string gitIgnorePath = PathUtils.Combine(rootPath, GitIgnoreFileName);
        
        HashSet<string> ignoredFiles = GetIgnoredFiles(gitIgnorePath, rootPath);
        
        List<string> files = Directory.GetFiles(rootPath, searchPattern, SearchOption.AllDirectories)
            .Select(Path.GetFullPath)
            .Where(file => !ignoredFiles.Contains(file))
            .ToList();

        return files;
    }
}