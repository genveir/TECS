using System;
using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.Chips;

internal class Nand : INotifyable
{
    private readonly Pin _a;
    private readonly Pin _b;
    private readonly Pin _output;
    
    public Nand(Pin a, Pin b, Pin output)
    {
        _a = a;
        _b = b;
        _output = output;
        
        _a.Connect(this);
        _b.Connect(this);
        
        Notify();
    }

    private bool[] CalculateValue() => new[] { !(_a.Value[0] && _b.Value[0]) };

    public void Notify()
    {
        _output.Value = CalculateValue();
    }
}