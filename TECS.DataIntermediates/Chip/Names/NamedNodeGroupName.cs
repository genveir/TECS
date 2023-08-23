using System;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Chip.Names;

public class NamedNodeGroupName
{
    public readonly string Value;

    private const string NodeGroupNameRegex = @"^\D+[a-zA-Z0-9]*(?:\[\d+\])?$";

    public NamedNodeGroupName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("named node group name can not be empty");

        if (!Regex.IsMatch(value, NodeGroupNameRegex))
            throw new ArgumentException("name {value} is not a valid node group name");
        
        Value = value;
    }
}