using TECS.HDLSimulator.Chips;

namespace TECS.Tests.Builders;

internal class NandBuilder
{
    private readonly Pin _a;
    private readonly Pin _b;
    private readonly Pin _output;
    
    public NandBuilder(Pin a, Pin b, Pin output)
    {
        _a = a;
        _b = b;
        _output = output;
    }

    public Nand Build()
    {
        return new(_a, _b, _output);
    }
}