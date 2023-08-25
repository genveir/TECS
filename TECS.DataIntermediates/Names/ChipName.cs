using System;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Names;

public class ChipName : TypedName
{
    public ChipName(string value) : base(value)
    {
        if (!Regex.IsMatch(value, $"^{RegularNameRegex}$"))
            throw new ArgumentException($"{value} contains invalid characters or starts with a digit");
    }
}