using System;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Chip.Names;

public class NamedNodeGroupName : TypedName
{
    private const string NodeGroupNameRegex = @$"^{RegularNameRegex}(?:\[\d+\])?$";

    public NamedNodeGroupName(string value) : base(value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("named node group name can not be empty");

        if (value == "true" || value == "false")
            throw new ArgumentException($"{value} is not a valid node group name");
        
        if (!Regex.IsMatch(value, NodeGroupNameRegex))
            throw new ArgumentException($"name {value} is not a valid node group name");
    }
}