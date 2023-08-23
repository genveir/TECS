namespace TECS.FileAccess.FileAccessors;

public class TestFile : FileAccessor
{
    public readonly string Name;
    
    public TestFile(string path) : base(path)
    {
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    }
    
    public string[] GetContents() => GetLines();

    public override string ToString()
    {
        return $"TestFile {Name}";
    }
}