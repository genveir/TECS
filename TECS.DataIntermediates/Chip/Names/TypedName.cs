using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace TECS.DataIntermediates.Chip.Names;

public abstract class TypedName
{
    public readonly string Value;

    protected const string RegularNameRegex = @"\D[a-zA-Z0-9]*";
    
    protected TypedName(string value)
    {
        if (string.IsNullOrWhiteSpace(value)) 
            throw new ArgumentException("typed name can not be empty");

        if (Char.IsNumber(value[0]))
            throw new ArgumentException("typed name can not start with a number");
        
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