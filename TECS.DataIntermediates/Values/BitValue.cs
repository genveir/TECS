using System;
using System.Linq;

namespace TECS.DataIntermediates.Values;

public class BitValue : INumberValue, IStringFormattableValue
{
    public bool[] Value { get; }
    
    public BitSize Size { get; }

    public static BitValue True => new(new[] { true });

    public static BitValue False => new(new[] { false });

    public BitValue(long value) : this(Convert.ToString(value, 2))
    { }

    public BitValue(bool value) : this(new[] { value }) 
    { }

    public BitValue(string binaryString) : this(binaryString
        .Select(c => c == '1')
        .Reverse()
        .ToArray()) { } 
    
    public BitValue(bool[] value)
    {
        if (value.Length == 0)
            throw new ArgumentException("bit value can not be empty");

        if (value.Length > 16)
            throw new ArgumentException("bit value is probably too long, can not be larger than 16 right now");

        Value = value;
        Size = new(value.Length);
    }

    public string AsBinaryString() => 
        new(Value.Reverse().Select(b => b ? '1' : '0').ToArray());

    public BitValue AsBitValue() => this;

    public ShortValue AsShortValue() => new ShortValue(this);
    
    public string FormatForOutput()
    {
        return AsBinaryString();
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

    public override string ToString()
    {
        return "BitValue " + AsBinaryString();
    }
}