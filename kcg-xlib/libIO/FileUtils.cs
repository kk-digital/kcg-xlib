using System.Diagnostics;
using assert;
using LogUtility;
using UtilityIO;

namespace FileLib;

public class FileUtils
{
    public static long ReadCounter;

    public static long FileReadStat;

    // TODO(): Must be multi threaded
    public static Stopwatch Stopwatch = new();

    // Todo: Temporary solution needs refactoring.
    /// Used when find directory is used...
    public static bool FileExists(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return false;
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        return System.IO.File.Exists(filePath);
    }

    public static bool DirectoryExists(string directoryPath)
    {
        Utils.Assert(!string.IsNullOrEmpty(directoryPath), "Directory path cannot be null or empty");
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        return Directory.Exists(directoryPath);
    }

    public static void CreateDirectory(string directoryPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        Directory.CreateDirectory(directoryPath);
    }

    public static void SetCurrentDirectory(string directoryPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        Directory.SetCurrentDirectory(directoryPath);
    }

    public static bool IsDirectoryEmpty(string directoryPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        return !Directory.EnumerateFileSystemEntries(directoryPath).Any();
    }

    public static void WriteAllBytes(string filePath, byte[] data)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        System.IO.File.WriteAllBytes(filePath, data);
    }

    public static void AppendText(string filePath, string text)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        System.IO.File.AppendAllText(filePath, text);
    }

    public static byte[] ReadAllBytes(string filePath)
    {
        string fixedFilePath = PathUtils.ReplaceBackSlashesWithForwardSlashes(filePath);
        Utils.Assert(FileExists(fixedFilePath), $"File not found: {fixedFilePath}");
        Utils.Assert(PathUtils.IsAbsolutePath(fixedFilePath),
            $"Only absolute paths are accepted. provided path: {fixedFilePath}");
        
        // timing the read functions and adding difference to counter
        Stopwatch.Reset();
        Stopwatch.Start();

        // file operation here
        byte[] result = System.IO.File.ReadAllBytes(fixedFilePath);

        Stopwatch.Stop();
        // adding the elpased ticks to ReadCounter
        ReadCounter += Stopwatch.ElapsedTicks;

        // increment the number of file reads
        FileReadStat++;

        return result;
    }

    public static string[] ReadAllLines(string filePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        // timing the read functions and adding difference to counter
        Stopwatch.Reset();
        Stopwatch.Start();

        // file operation here
        string[] result = System.IO.File.ReadAllLines(filePath);

        Stopwatch.Stop();
        // adding the elpased ticks to ReadCounter
        ReadCounter += Stopwatch.ElapsedTicks;

        // increment the number of file reads
        FileReadStat++;

        return result;
    }

    public static string ReadAllText(string filePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        // timing the read functions and adding difference to counter
        Stopwatch.Reset();
        Stopwatch.Start();

        // file operation here
        string result = System.IO.File.ReadAllText(filePath);

        Stopwatch.Stop();
        // adding the elpased ticks to ReadCounter
        ReadCounter += Stopwatch.ElapsedTicks;

        // increment the number of file reads
        FileReadStat++;

        return result;
    }

    public static string GetDirectoryFromFilepath(string filePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        // Get the directory (folder) path from the full file path
        string folderPath = PathUtils.GetDirectoryName(filePath).Replace('\\', '/');

        return folderPath;
    }

    // returns the directory paths of the sub directories
    public static string[] GetDirectories(string directoryPath, string searchPattern, SearchOption option)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        var searchOption = System.IO.SearchOption.AllDirectories;

        switch (option)
        {
            case SearchOption.AllDirectories:
            {
                searchOption = System.IO.SearchOption.AllDirectories;
                break;
            }
            case SearchOption.TopDirectoryOnly:
            {
                searchOption = System.IO.SearchOption.TopDirectoryOnly;
                break;
            }
        }

        ;

        // Get directories and replace backslashes with forward slashes
        string[] directories = Directory.GetDirectories(directoryPath, searchPattern, searchOption);
        for (var i = 0; i < directories.Length; i++)
            directories[i] = directories[i].Replace('\\', '/');

        return directories;
    }

    /// returns the file paths of the files in the directory
    public static string[] GetFiles(string directoryPath, string searchPattern, SearchOption option)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        System.IO.SearchOption searchOption = System.IO.SearchOption.AllDirectories;

        switch (option)
        {
            case SearchOption.AllDirectories:
            {
                searchOption = System.IO.SearchOption.AllDirectories;
                break;
            }
            case SearchOption.TopDirectoryOnly:
            {
                searchOption = System.IO.SearchOption.TopDirectoryOnly;
                break;
            }
        }

        ;

        List<string> correctedPaths = new();
        // directory get directories sometimes returns \ as a path divider which we don't want
        foreach (string path in Directory.GetFiles(directoryPath, searchPattern, searchOption))
            correctedPaths.Add(path.Replace('\\', '/'));

        return correctedPaths.ToArray();
    }

    public static void ClearDirectory(string directoryPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        foreach (string filePath in Directory.GetFiles(directoryPath))
            System.IO.File.Delete(filePath);

        foreach (string subdirectoryPath in Directory.GetDirectories(directoryPath))
            Directory.Delete(subdirectoryPath, true);
    }
    
    public static void DeleteFilesOfTypeInDirectory(string directoryPath, string fileType)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");

        string[] files = GetFiles(directoryPath,  $"*.{fileType}", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            DeleteFile(file);
        }
    }


    public static void DeleteDirectory(string directoryPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        // Check if the directory exists before attempting to delete
        if (Directory.Exists(directoryPath))
            Directory.Delete(directoryPath, true);
    }

    public static void MoveDirectory(string sourceFolderPath, string destinationFolderPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(sourceFolderPath),
            $"Only absolute paths are accepted. provided path: {sourceFolderPath}");
        Utils.Assert(PathUtils.IsAbsolutePath(destinationFolderPath),
            $"Only absolute paths are accepted. provided path: {destinationFolderPath}");

        // Check if the source folder exists
        if (DirectoryExists(sourceFolderPath))
            // Rename the folder
            Directory.Move(sourceFolderPath, destinationFolderPath);
    }

    public static long GetFileLastWriteTime(string filePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");

        // Get the last modify time of the file
        FileInfo fileInfo = new (filePath);

        return fileInfo.LastWriteTime.Ticks;
    }

    public static float GetFileReadTimeInSeconds()
    {
        TimeSpan timeSpan = new (ReadCounter);

        return (float)timeSpan.TotalSeconds;
    }
    
    public static void WriteAllText(string filePath, string content)
    {
        string directory = PathUtils.GetDirectoryName(filePath);
        CreateDirectoryIfDoesNotExists(directory);

        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        File.WriteAllText(filePath, content);
    }
    
    public static void WriteToFile(string filePath, string content)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
            // Open the file for writing using a StreamWriter
        using StreamWriter writer = new (filePath, true);
        writer.WriteLine(content);
    }

    public static void DeleteFile(string filePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        try
        {
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            else
            {
                LibLog.LogWarning($"File not found: {filePath}");
            }
        }
        catch (Exception ex)
        {
            LibLog.LogError($"An error occurred while deleting the file: {filePath}. Error: {ex.Message}");
        }
    }
    
    public static void RenameFile(string filePath, string newFilePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(filePath),
            $"Only absolute paths are accepted. provided path: {filePath}");
        try
        {
            if (File.Exists(filePath))
            {
                File.Move(filePath, newFilePath);
            }
            else
            {
                LibLog.LogWarning($"File not found: {filePath}.");
            }
        }
        catch (Exception ex)
        {
            LibLog.LogError($"An error occurred while deleting the file: {filePath}. Error: {ex.Message}");
        }
    }
    
    public static void CopyFile(string sourceFilePath, string destinationFilePath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(sourceFilePath),
            $"Only absolute paths are accepted. provided path: {sourceFilePath}");
        Utils.Assert(PathUtils.IsAbsolutePath(destinationFilePath),
            $"Only absolute paths are accepted. provided path: {destinationFilePath}");
        
        string destinationDirectory = PathUtils.GetDirectoryName(destinationFilePath);
        CreateDirectoryIfDoesNotExists(destinationDirectory);
        
        File.Copy(sourceFilePath, destinationFilePath, true);
    }

    public static string GetFileNameWithoutExtension(string filePath)
    {
        string fixedFilePath = PathUtils.ReplaceBackSlashesWithForwardSlashes(filePath);
        Utils.Assert(PathUtils.IsAbsolutePath(fixedFilePath),
            $"Only absolute paths are accepted. provided path: {fixedFilePath}");
        List<string> pathArray = fixedFilePath.Split('\\', '/').ToList();
        List<string> last = pathArray.Last().Split(".").ToList();
        last.RemoveAt(last.Count - 1);
        return string.Join(".", last);
    }

    /// <summary>
    ///     Ensures that file exists in a given directory (directory creates directory if it doesn't exist)
    /// </summary>
    /// <param name="directoryPath">Path to file directory</param>
    /// <param name="fileName">file name with extension</param>
    public static void EnsureFileExists(string directoryPath, string fileName)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        CreateDirectory(directoryPath);
        string filePath = PathUtils.Combine(directoryPath, fileName);
        if (FileExists(filePath)) return;

        System.IO.File.WriteAllText(filePath, "");
    }

    public static void CreateDirectoryIfDoesNotExists(string directory)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directory),
            $"Only absolute paths are accepted. provided path: {directory}");
        if (!DirectoryExists(directory))
            CreateDirectory(directory);
    }

    public static void SetProjectDirectory(string directoryPath)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(directoryPath),
            $"Only absolute paths are accepted. provided path: {directoryPath}");
        Utils.Assert(DirectoryExists(directoryPath), $"Couldn't find directory at {directoryPath}");
        Constants.ProjectDirectory = directoryPath;
        SetCurrentDirectory(Constants.ProjectDirectory);
    }
    
    public static FileStream OpenWrite(string path)
    {
        Utils.Assert(PathUtils.IsAbsolutePath(path),
            $"Only absolute paths are accepted. provided path: {path}");
        return new FileStream(path, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None);
    }
}