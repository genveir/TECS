using System;

namespace TECS.DataIntermediates.Names;

public class BitSize
{
    public int Value { get; }

    public BitSize(int value)
    {
        if (value < 1)
            throw new ArgumentException("bit size can not be 0 or empty");

        if (value > 64)
            throw new ArgumentException("bit size is probably too high, maxed at 64 right now");

        Value = value;
    }
}