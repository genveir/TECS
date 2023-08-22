using System.IO;

namespace TECS.HDLSimulator.HDL;

internal class HdlFile
{
    private readonly string _path;
    private string? _fileContents;
    
    public HdlFile(string path)
    {
        _path = path;
    }

    private string ReadFile()
    {
        using var reader = new StreamReader(new FileStream(_path, FileMode.Open));

        return reader.ReadToEnd();
    }

    public string GetContents()
    {
         _fileContents ??= ReadFile();

        return _fileContents;
    }
}