using TECS.HDLSimulator.Chips;

namespace TECS.Tests.Builders;

internal class PinBuilder
{
    private readonly int _bitSize;
    
    private bool[]? _value;
    
    public PinBuilder(int bitSize)
    {
        _bitSize = bitSize;
    }

    public PinBuilder WithValue(params bool[] value)
    {
        _value = value;
        
        return this;
    }

    public Pin Build()
    {
        var pin = new Pin(_bitSize);
        if (_value != null) pin.Value = _value;
        
        return pin;
    }
}