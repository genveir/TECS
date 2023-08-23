using System;
using System.Linq;

namespace TECS.DataIntermediates.Chip.Names;

public abstract class TypedName
{
    public readonly string Value;

    protected TypedName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("typed name can not be empty");

        if (value.Any(Char.IsWhiteSpace))
            throw new ArgumentException("typed name can not contain whitespace");
        if (value.Contains(','))
            throw new ArgumentException("typed name {value} can not contain a comma");
        if (value.Contains(';'))
            throw new ArgumentException("typed name {value} can not contain a semicolon");

        Value = value;
    }

    public override string ToString()
    {
        return Value;
    }
}