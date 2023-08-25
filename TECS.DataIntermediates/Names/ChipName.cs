using System;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Names;

public class ChipName : TypedName
{
    internal ChipName(string value) : base(value)
    {
        if (!Regex.IsMatch(value, $"^{RegularNameRegex}$"))
            throw new ArgumentException($"{value} contains invalid characters or starts with a digit");
    }
    
    public override int GetHashCode()
    {
        return string.GetHashCode(Value, StringComparison.Ordinal) + 4;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as ChipName;

        if (other == null) return false;
        return other.Value.Equals(Value);
    }
}