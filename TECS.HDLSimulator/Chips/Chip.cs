using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.Chips;

public class Chip
{
    private readonly Dictionary<string, Pin> _inputs;
    private readonly Dictionary<string, Pin> _outputs;
    
    public string Name { get; }

    internal Chip(string name, Dictionary<string, Pin> inputs, Dictionary<string, Pin> outputs)
    {
        Name = name;
        
        _inputs = inputs;
        _outputs = outputs;
    }

    public void SetInput(string name, bool[] value) => _inputs[name].Value = value;

    public bool[] ReadOutput(string name) => _outputs[name].Value;
}

