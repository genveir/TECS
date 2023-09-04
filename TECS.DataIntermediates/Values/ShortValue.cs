using System;

namespace TECS.DataIntermediates.Values;

public class ShortValue : INumberValue, IStringFormattableValue
{
    public short Value { get; }
    
    public ShortValue(short value)
    {
        Value = value;
    }

    public ShortValue(BitValue bitValue) : this(
        Convert.ToInt16(
            value: bitValue.AsBinaryString(), 
            fromBase: 2))
    {

    }

    public BitValue AsBitValue() => new(Value);

    public ShortValue AsShortValue() => this;

    public string FormatForOutput() => Value.ToString();
}