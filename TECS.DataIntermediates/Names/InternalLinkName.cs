using System;

namespace TECS.DataIntermediates.Names;

public class InternalLinkName : LinkName
{
    internal InternalLinkName(string value) : base(value)
    {
        if (value =="true")
            throw new ArgumentException("internal pin can not be true");
        if (value =="false")
            throw new ArgumentException("internal pin can not be false");
    }
    
    public override int GetHashCode()
    {
        return string.GetHashCode(Value, StringComparison.Ordinal) + 2;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as InternalLinkName;

        if (other == null) return false;
        return other.Value.Equals(Value);
    }
}