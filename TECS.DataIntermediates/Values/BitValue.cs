using System;
using System.Linq;

namespace TECS.DataIntermediates.Values;

public class BitValue : INumberValue, IStringFormattableValue
{
    public bool[] Value { get; }
    
    public BitSize Size { get; }

    public static BitValue True => new(true);

    public static BitValue False => new(false);

    public BitValue(bool value) : this(new[] { value }, new(1)) 
    { }
    
    public BitValue(short value, int size) : this(Convert.ToString(value, 2), size)
    { }

    public BitValue(string binaryString, int size) : this(binaryString
        .Replace("%B", "")
        .Select(c =>
        {
            if (c == '1') return true;
            if (c == '0') return false;
            else throw new InvalidOperationException($"{binaryString} is not a binary string");
        })
        .Reverse()
        .ToArray(), new(size)) { } 
    
    public BitValue(bool[] value, BitSize size)
    {
        if (value.Length == 0)
            throw new ArgumentException("bit value can not be empty");

        if (value.Length > 16)
            throw new ArgumentException("bit value is probably too long, can not be larger than 16 right now");

        if (value.Length > size.Value)
            throw new ArgumentException("passed value is larger than the alotted size");

        var wellSizedValue = new bool[size.Value];
        for (int n = 0; n < value.Length; n++)
            wellSizedValue[n] = value[n];        
            
        Value = wellSizedValue;
        Size = size;
    }

    public string AsBinaryString() => 
        new(Value.Reverse().Select(b => b ? '1' : '0').ToArray());

    public BitValue AsBitValue() => this;

    public ShortValue AsShortValue() => new(this);
    
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