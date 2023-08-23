namespace TECS.FileAccess.FileAccessors;

public class ComparisonFile : FileAccessor
{
    public readonly string Name;
    
    public ComparisonFile(string path) : base(path)
    {
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    }
    
    public string[] GetContents() => GetLines();

    public override string ToString()
    {
        return $"ComparisonFile {Name}";
    }
}