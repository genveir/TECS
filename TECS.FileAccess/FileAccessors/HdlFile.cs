namespace TECS.FileAccess.FileAccessors;

public class HdlFile : FileAccessor
{
    private readonly string _name;

    public HdlFile(string path) : base(path)
    {
        _name = System.IO.Path.GetFileNameWithoutExtension(path);
    }

    public string[] GetContents() => GetLines();

    public override string ToString()
    {
        return $"HdlFile {_name}";
    }
}