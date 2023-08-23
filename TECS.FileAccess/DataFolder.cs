using System.IO;

namespace TECS.FileAccess;

public class DataFolder
{
    private readonly string _path;
    
    public DataFolder(string path)
    {
        _path = path;
    }

    public HdlFolder HdlFolder => new HdlFolder(Path.Join(_path, "HDL"));
}