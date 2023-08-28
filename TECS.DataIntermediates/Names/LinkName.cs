using System;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Names;

public class LinkName : TypedName
{
    private const string BitBlockIndicatorRegex = @"^\[\d+(?:\.\.\d+)*\]$";
    
    public LinkName(string value) : base(value)
    {
        if (value.Contains('['))
        {
            if (value.StartsWith("true") || value.StartsWith("false"))
                throw new ArgumentException("true and false can not have a bit indicator block");

            if (value[0] == '[')
                throw new ArgumentException("link name can not start with bit indicator block");
            if (!value.EndsWith(']'))
                throw new ArgumentException("bit indicator block must end at the end of link name");

            var bitIndicatorBlock = value.Substring(value.IndexOf('['));

            var match = Regex.Match(bitIndicatorBlock, BitBlockIndicatorRegex);
            if (!match.Success)
                throw new ArgumentException($"{bitIndicatorBlock} is an invalid bit indicator block");
        }
        else if (value.Contains(']'))
            throw new ArgumentException("link name cannot have a closing bracket without an opening bracket");
    }
}