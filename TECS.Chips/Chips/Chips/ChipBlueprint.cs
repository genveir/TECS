using System.Collections.Generic;
using TECS.DataIntermediates.Names;

namespace TECS.HDLSimulator.Chips.Chips;

public class ChipBlueprint
{
    private readonly ChipName _name;
    public Dictionary<NamedNodeGroupName, NamedInputNodeGroup> Inputs { get; }
    
    public Dictionary<NamedNodeGroupName, NamedOutputNodeGroup> Outputs { get; }

    public ChipBlueprint(ChipName name, 
        Dictionary<NamedNodeGroupName, NamedInputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, NamedOutputNodeGroup> outputs)
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