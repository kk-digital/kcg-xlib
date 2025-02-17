# lib-io Library README

## Library Description

The `lib-io` library is a powerful utility library designed to facilitate file and path operations in C# applications. It aims to simplify and enhance how developers handle file management tasks, providing a streamlined, reliable, and efficient interface for interacting with the file system.

## Functionality

### FileUtils

- **FileExists(string filePath)**: Checks if the file exists at the given absolute path.
- **DirectoryExists(string directoryPath)**: Checks if the directory exists at the given absolute path.
- **CreateDirectory(string directoryPath)**: Creates a directory at the specified path.
- **SetCurrentDirectory(string directoryPath)**: Sets the current working directory.
- **IsDirectoryEmpty(string directoryPath)**: Checks if the directory is empty.
- **WriteAllBytes(string filePath, byte[] data)**: Writes a byte array to the specified file path.
- **AppendText(string filePath, string text)**: Appends text to a file.
- **ReadAllBytes(string filePath)**: Reads all bytes from the specified file path.
- **ReadAllLines(string filePath)**: Reads all lines from the specified file path.
- **ReadAllText(string filePath)**: Reads all text from the specified file path.
- **GetDirectoryFromFilePath(string filePath)**: Extracts the directory from a full file path.
- **GetDirectories(string directoryPath, string searchPattern, SearchOption option)**: Retrieves directories based on search patterns.
- **GetFiles(string directoryPath, string searchPattern, SearchOption option)**: Retrieves files based on search patterns.
- **ClearDirectory(string directoryPath)**: Clears all contents of a directory.
- **DeleteFilesOfTypeInDirectory(string directoryPath, string fileType)**: Deletes specific file types from the directory.
- **DeleteDirectory(string directoryPath)**: Deletes the specified directory.
- **MoveDirectory(string sourceFolderPath, string destinationFolderPath)**: Moves a directory to a new location.
- **GetFileLastWriteTime(string filePath)**: Gets the last modified time of a file.
- **GetFileReadTimeInSeconds()**: Gets the accumulated read time for files in seconds.
- **WriteAllText(string filePath, string content)**: Writes all text to a file.
- **WriteToFile(string filePath, string content)**: Appends text to a file.
- **DeleteFile(string filePath)**: Deletes the file at the specified path.
- **RenameFile(string filePath, string newFilePath)**: Renames the file to a new name.
- **CopyFile(string sourceFilePath, string destinationFilePath)**: Copies a file from the source to the destination.
- **GetFileNameWithoutExtension(string filePath)**: Retrieves the file name without its extension.
- **EnsureFileExists(string directoryPath, string fileName)**: Ensures that a file exists in the specified directory.
- **CreateDirectoryIfDoesNotExists(string directory)**: Creates a directory if it does not exist.
- **SetProjectDirectory(string directoryPath)**: Sets the project's main directory.
- **OpenWrite(string path)**: Opens a file for writing or creates it if it doesn't exist.

### PathUtils

- **GetWorkingDirectory()**: Retrieves the current working directory.
- **GetTemporaryDirectory()**: Retrieves the system's temporary directory.
- **IsAbsolutePath(string path)**: Checks if the given path is absolute.
- **ReplaceBackSlashesWithForwardSlashes(string path)**: Replaces backslashes with forward slashes.
- **GetRelative(string path, string rootDirectory)**: Retrieves the relative path.
- **GetFullPath(string path)**: Retrieves the full path.
- **GetAbsolutePath(string baseDirectory, string relativePath)**: Constructs an absolute path.
- **IsValidAbsolutePath(string path)**: Validates if the path is a valid absolute path.
- **IsValidRelativePath(string path)**: Validates if the path is a valid relative path.
- **GetParentPath(string fullPath)**: Retrieves the parent path.
- **Combine(params string[] paths)**: Combines multiple paths.
- **Join(params string[] paths)**: Joins multiple paths.
- **GetFileName(string path)**: Retrieves the file name.
- **GetDirectoryName(string path)**: Retrieves the directory name.
- **GetInvalidFileNameChars()**: Retrieves invalid characters for file names.

## Usage Instructions

```csharp
using FileLib;
using UtilityIO;

// Checking if a file exists
bool fileExists = FileUtils.FileExists("/absolute/path/to/file.txt");

// Creating a directory
FileUtils.CreateDirectory("/absolute/path/to/new/directory");

// Reading file content
string fileContent = FileUtils.ReadAllText("/absolute/path/to/file.txt");

// Getting the working directory
string workingDirectory = PathUtils.GetWorkingDirectory();
```

## Dependencies

- lib-assert: For assertion checks.
- lib-log: For logging purposes.
- lib-os: For operating system utilities.