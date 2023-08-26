using System;

namespace TECS.FileAccess.FileAccessors;

public class ComparisonFile : FileAccessor
{
    public readonly string Name;
    
    public ComparisonFile(string path) : base(path)
    {
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    }
    
    public string[] GetContents() => GetLines();
    
    public override int GetHashCode()
    {
        return string.GetHashCode(Name, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as ComparisonFile;

        if (other == null) return false;
        return other.Name.Equals(Name);
    }

    public override string ToString()
    {
        return $"ComparisonFile {Name}";
    }
}