using System;

namespace TECS.FileAccess.FileAccessors;

public class HdlFile : FileAccessor
{
    private readonly string _path;
    public readonly string Name;

    public HdlFile(string path) : base(path)
    {
        _path = path;
        Name = System.IO.Path.GetFileNameWithoutExtension(path);
    }

    public string[] GetContents() => GetLines();

    public override int GetHashCode()
    {
        return string.GetHashCode(_path, StringComparison.Ordinal);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as HdlFile;

        if (other == null) return false;
        return other._path.Equals(_path);
    }

    public override string ToString()
    {
        return $"HdlFile {Name}";
    }
}