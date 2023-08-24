namespace TECS.FileAccess.FileAccessors;

public class HdlFile : FileAccessor
{
    public readonly string Name;

    public HdlFile(string path) : base(path)
    {
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    }

    public string[] GetContents() => GetLines();

    public override string ToString()
    {
        return $"HdlFile {Name}";
    }
}