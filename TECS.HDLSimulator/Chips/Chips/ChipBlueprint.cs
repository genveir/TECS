using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.Chips;

public class ChipBlueprint
{
    public string Name { get; }
    public Dictionary<string, NamedNodeGroup> Inputs { get; }
    
    public Dictionary<string, NamedNodeGroup> Outputs { get; }

    public ChipBlueprint(string name, Dictionary<string, NamedNodeGroup> inputs, Dictionary<string, NamedNodeGroup> outputs)
    {
        Name = name;
        Inputs = inputs;
        Outputs = outputs;
    }

    public Chip Fabricate()
    {
        return new(Inputs, Outputs);
    }

    public override string ToString()
    {
        return $"ChipBlueprint {Name}";
    }
}