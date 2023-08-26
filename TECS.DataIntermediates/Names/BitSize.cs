using System;

namespace TECS.DataIntermediates.Names;

public class BitSize
{
    public int Value { get; }

    internal BitSize(int value)
    {
        if (value < 1)
            throw new ArgumentException("bit size can not be 0 or empty");

        if (value > 64)
            throw new ArgumentException("bit size is probably too high, maxed at 64 right now");

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