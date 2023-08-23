using System.IO;

namespace TECS.FileAccess.FileAccessors;

public abstract class FileAccessor
{
    protected readonly string Path;
    protected string[]? FileContents;
    
    protected FileAccessor(string path)
    {
        Path = path;
    }

    private string[] ReadLines()
    {
        return File.ReadAllLines(Path);
    }

    protected string[] GetLines()
    {
        FileContents ??= ReadLines();

        return FileContents;
    }    
}