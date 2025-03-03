using assert;
using OperatingSystem;
using OperatingSystemEnums;

namespace UtilityIO;

public static class PathUtils
{
    private static readonly HashSet<char> AllowedSpecialChars = new()
    {
        '_', '-', '.', '/',
        ' ' /*TODO: this is temporary until we fix tileset folders containing spaces*/
    };

    
    public static string GetWorkingDirectory()
    {
        string currentDirectory = Directory.GetCurrentDirectory();

        currentDirectory = ReplaceBackSlashesWithForwardSlashes(currentDirectory);

        return currentDirectory;
    }

    public static string GetTemporaryDirectory()
    {
        // Get the system temporary folder
        string tempPath = Path.GetTempPath();
        tempPath = ReplaceBackSlashesWithForwardSlashes(tempPath);
        
        return tempPath;
    }
    
    public static bool IsAbsolutePath(string path)
    {
        return IsValidAbsolutePath(path);
    }

    public static string ReplaceBackSlashesWithForwardSlashes(string path)
    {
        return path.Replace('\\', '/');
    }

    public static string GetRelative(string path, string rootDirectory)
    {
        AssertAbsolutePath(path);
        AssertPathStartsWithRoot(path, rootDirectory);

        string relativePath = path.Substring(rootDirectory.Length);
        return relativePath.StartsWith('/') ? relativePath.Substring(1) : relativePath;
    }
    
    public static string GetFullPath(string path)
    {
        return Path.GetFullPath(path).Replace('\\', '/');
    }

    public static string GetAbsolutePath(string baseDirectory, string relativePath)
    {
        AssertAbsolutePath(baseDirectory);
        string fullPath = Combine(baseDirectory, relativePath);
        AssertValidAbsolutePath(fullPath);
        return fullPath;
    }
    
    #nullable enable
    public static bool IsValidAbsolutePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;
        
        if (OperatingSystemUtils.GetOperatingSystem() == OperatingSystemType.Windows)
        {
            if (!IsValidWindowsPath(path))
            {
                return false;
            }
            path = path.Substring(3);
        }
        else
        {
            if (!path.StartsWith('/'))
            {
                return false;
            }
            path = path.Substring(1);
        }

        for (int characterIndex = 0; characterIndex < path.Length; characterIndex++)
        {
            char c = path[characterIndex];
            
            bool isLetterOrDigit = IsLetterOrDigit(c);
            bool containsAllowedCharacters = AllowedSpecialChars.Contains(c);
            
            if (!isLetterOrDigit && !containsAllowedCharacters)
            {
                return false;
            }
        }

        return true;
    }

    public static bool IsLetterOrDigit(char c)
    {
        return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }

    #nullable enable
    public static bool IsValidRelativePath(string path)
    {
        if (string.IsNullOrEmpty(path)) return false;

        if (path.StartsWith('/')) return false;

        foreach (char c in path)
            if (!char.IsLetterOrDigit(c) && !AllowedSpecialChars.Contains(c))
                return false;

        return true;
    }

    private static bool IsValidWindowsPath(string path)
    {
        // Additional check for Windows paths to ensure they start with a drive letter and colon
        int pathLength = path.Length;
        if (pathLength < 3) return false;
        
        bool firstCharIsLetter = char.IsLetter(path[0]);
        bool secondCharIsColon = path[1] == ':';
        bool thirdCharIsForwardSlash = path[2] == '/';
        return  firstCharIsLetter && secondCharIsColon && thirdCharIsForwardSlash;
    }

    public static string GetParentPath(string fullPath)
    {
        AssertAbsolutePath(fullPath);
        AssertValidAbsolutePath(fullPath);
        return fullPath.Substring(0, fullPath.LastIndexOf('/'));
    }

    public static string Combine(string path1, string path2)
    {
        ValidatePath(path1);
        ValidatePath(path2);
        return Path.Combine(path1, path2).Replace('\\', '/');
    }

    public static string GetFileExtension(string path)
    {
        ValidatePath(path);
        
        string extension = Path.GetExtension(path);

        return extension;
    }
    public static string Join(string path1, string path2)
    {
        ValidatePath(path1);
        ValidatePath(path2);
        return Path.Join(path1, path2).Replace('\\', '/');
    }

    public static string Join(string path1, string path2, string path3)
    {
        ValidatePath(path1);
        ValidatePath(path2);
        ValidatePath(path3);
        return Path.Join(path1, path2, path3).Replace('\\', '/');
    }
    
    public static string Join(string path1, string path2, string path3, string path4)
    {
        ValidatePath(path1);
        ValidatePath(path2);
        ValidatePath(path3);
        ValidatePath(path4);
        return Path.Join(path1, path2, path3, path4).Replace('\\', '/');
    }
    
    public static string Join(params string[] paths)
    {
        foreach (string path in paths)
        {
            ValidatePath(path);
        }
        return Path.Join(paths).Replace('\\', '/');
    }
    
    /// The characters after the last directory separator character in path.
    /// If the last character of path is a directory or volume separator character, this method returns Empty.
    public static string GetFileName(string path)
    {
        ValidatePath(path);
        return Path.GetFileName(path);
    }
    
    /// Returns Directory information for path,
    /// or Empty if path denotes a root directory.
    /// Returns Empty if path does not contain directory information.
    public static string GetDirectoryName(string path)
    {
        ValidatePath(path);
        string? dir = Path.GetDirectoryName(path);
        return dir == null ? "" : dir.Replace('\\', '/');
    }

    public static char[] GetInvalidFileNameChars()
    {
        return Path.GetInvalidFileNameChars();
    }
    
    #region Path Assertions

    private static void AssertAbsolutePath(string path)
    {
        Utils.Assert(IsAbsolutePath(path), $"Only absolute paths are accepted. provided path: {path}");
    }

    private static void AssertPathStartsWithRoot(string path, string rootDirectory)
    {
        Utils.Assert(path.StartsWith(rootDirectory),
            $"Only paths which start with root directory {rootDirectory} are allowed, provided: {path}");
    }

    private static void AssertValidAbsolutePath(string path)
    {
        Utils.Assert(IsValidAbsolutePath(path),
            $"Paths should only contain _, -, ., lower case and upper case letters, and numbers. " +
            $"No unicode characters. Provided path: {path}");
    }
    
    private static void AssertValidRelativePath(string path)
    {
        Utils.Assert(IsValidRelativePath(path),
            $"Relative paths should no start at '/', only contain _, -, ., lower case and upper case letters, " +
            $"and numbers. No unicode characters. Provided path: {path}");
    }
    
    private static void ValidatePath(string path)
    {
        if (IsAbsolutePath(path))
            AssertValidAbsolutePath(path);
        else
            AssertValidRelativePath(path);
    }

    #endregion
}