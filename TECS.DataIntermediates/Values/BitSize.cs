using System;

namespace TECS.DataIntermediates.Values;

public class BitSize
{
    public int Value { get; }

    public BitSize(int value)
    {
        if (value < 1)
            throw new ArgumentException("bit size can not be 0 or empty");

        if (value > 16)
            throw new ArgumentException("bit size is probably too high, maxed at 16 right now");

        Value = value;
    }

    public override string ToString()
    {
        return $"BitSize {Value}";
    }

    public override int GetHashCode() => Value + 5;

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(this, obj)) return true;
        var other = obj as BitSize;

        if (other == null) return false;
        return other.Value == Value;
    }
}