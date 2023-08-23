using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.Chips;

public class ChipBlueprint
{
    private readonly string _name;
    public Dictionary<string, NamedNodeGroup> Inputs { get; }
    
    public Dictionary<string, NamedNodeGroup> Outputs { get; }

    public ChipBlueprint(string name, Dictionary<string, NamedNodeGroup> inputs, Dictionary<string, NamedNodeGroup> outputs)
    {
        _name = name;
        Inputs = inputs;
        Outputs = outputs;
    }

    public Chip Fabricate()
    {
        return new(Inputs, Outputs);
    }

    public override string ToString()
    {
        return $"ChipBlueprint {_name}";
    }
}