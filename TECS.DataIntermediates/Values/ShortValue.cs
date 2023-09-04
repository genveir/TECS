using System;

namespace TECS.DataIntermediates.Values;

public class ShortValue : INumberValue, IStringFormattableValue
{
    public short Value { get; }
    
    public BitSize Size { get; }
    
    public ShortValue(short value, int bitSize) : this(value, new BitSize(bitSize))
    { }

    public ShortValue(short value, BitSize bitSize)
    {
        Value = value;
        Size = bitSize;
    }

    public ShortValue(BitValue bitValue) : this(
        Convert.ToInt16(
            value: bitValue.AsBinaryString(), 
            fromBase: 2), bitValue.Size)
    {

    }

    public BitValue AsBitValue() => new(Value, Size.Value);

    public ShortValue AsShortValue() => this;

    public string FormatForOutput() => Value.ToString();
}