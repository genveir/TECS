using System;

namespace TECS.DataIntermediates.Names;

public class ExternalLinkName : LinkName
{
    internal ExternalLinkName(string value) : base(value) {}
    
    public override int GetHashCode()
    {
        return string.GetHashCode(Value, StringComparison.Ordinal) + 3;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as ExternalLinkName;

        if (other == null) return false;
        return other.Value.Equals(Value);
    }
}