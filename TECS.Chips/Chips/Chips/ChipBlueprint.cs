using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class ChipBlueprint
{
    private readonly ChipName _name;
    public Dictionary<NamedNodeGroupName, InputNodeGroup> Inputs { get; }
    
    public Dictionary<NamedNodeGroupName, OutputNodeGroup> Outputs { get; }

    public ChipBlueprint(ChipName name, 
        Dictionary<NamedNodeGroupName, InputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs)
    {
        _name = name;
        Inputs = inputs;
        Outputs = outputs;
    }

    public Chip Fabricate()
    {
        return new(_name, Inputs, Outputs);
    }

    public override string ToString()
    {
        return $"ChipBlueprint {_name}";
    }
}