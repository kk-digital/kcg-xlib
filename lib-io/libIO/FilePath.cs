using assert;
using UtilityIO;

namespace FileLib;

public class FilePath
{
    private readonly string _path;

    public FilePath(string path)
    {
        Utils.Assert(FileUtils.FileExists(path), $"File {path} does not exist");
        Utils.Assert(PathUtils.IsValidAbsolutePath(path), "Path is not absolute");
        _path = path;
    }

    public string GetPath()
    {
        return _path;
    }

    public bool Exists()
    {
        return FileUtils.FileExists(_path);
    }

    public void WriteAllBytes(byte[] data)
    {
        FileUtils.WriteAllBytes(_path, data);
    }

    public void AppendText(string text)
    {
        FileUtils.AppendText(_path, text);
    }

    public byte[] ReadAllBytes()
    {
        return FileUtils.ReadAllBytes(_path);
    }

    public string[] ReadAllLines()
    {
        return FileUtils.ReadAllLines(_path);
    }

    public string ReadAllText()
    {
        return FileUtils.ReadAllText(_path);
    }

    public string GetDirectoryFromFilepath()
    {
        return FileUtils.GetDirectoryFromFilepath(_path);
    }

    public long GetFileLastWriteTime()
    {
        return FileUtils.GetFileLastWriteTime(_path);
    }

    public void WriteAllText(string content)
    {
        FileUtils.WriteAllText(_path, content);
    }

    public void WriteToFile(string content)
    {
        FileUtils.WriteToFile(_path, content);
    }

    public void DeleteFile()
    {
        FileUtils.DeleteFile(_path);
    }

    public void RenameFile(string newFilePath)
    {
        FileUtils.RenameFile(_path, newFilePath);
    }

    public void CopyFile(string destinationFilePath)
    {
        FileUtils.CopyFile(_path, destinationFilePath);
    }

    public string GetFileNameWithoutExtension()
    {
        return FileUtils.GetFileNameWithoutExtension(_path);
    }

    public FileStream OpenWrite()
    {
        return FileUtils.OpenWrite(_path);
    }
}