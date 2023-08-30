using System;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Names;

public class NamedNodeGroupName : TypedName
{
    private const string NodeGroupNameRegex = @$"^{RegularNameRegex}$";
    
    public NamedNodeGroupName(string value, bool cannotBeBoolean = true) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("named node group name can not be empty");
        
        if (cannotBeBoolean && (value == "true" || value == "false"))
            throw new ArgumentException($"{value} is not a valid node group name");
        
        if (!Regex.IsMatch(value, NodeGroupNameRegex))
            throw new ArgumentException($"name {value} is not a valid node group name");
    }

    public override int GetHashCode()
    {
        return string.GetHashCode(Value, StringComparison.Ordinal) + 1;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as NamedNodeGroupName;

        if (other == null) return false;
        return other.Value.Equals(Value);
    }
}