using System;

namespace TECS.DataIntermediates.Values;

public class LongValue : INumberValue, IStringFormattableValue
{
    public long Value { get; }
    
    public LongValue(long value)
    {
        Value = value;
    }

    public LongValue(BitValue bitValue) : this(
        Convert.ToInt64(
            value: bitValue.AsBinaryString(), 
            fromBase: 2))
    {

    }

    public BitValue AsBitValue() => new(Value);

    public LongValue AsLongValue() => this;

    public string FormatForOutput() => Value.ToString();
}