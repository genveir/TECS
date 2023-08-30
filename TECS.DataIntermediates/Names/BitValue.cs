using System;
using System.Linq;

namespace TECS.DataIntermediates.Names;

public class BitValue
{
    public bool[] Value { get; }
    
    public BitSize Size { get; }

    public static BitValue True => new(new[] { true });

    public static BitValue False => new(new[] { false });
    
    public BitValue(bool[] value)
    {
        if (value.Length == 0)
            throw new ArgumentException("bit value can not be empty");

        if (value.Length > 64)
            throw new ArgumentException("bit value is probably too long, can not be larger than 64 right now");

        Value = value;
        Size = new(value.Length);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as BitValue;

        if (other == null) return false;
        return other.Value.SequenceEqual(Value);
    }
}