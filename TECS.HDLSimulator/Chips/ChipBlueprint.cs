using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprint
{
    public string Name { get; }
    public Dictionary<string, NandPinNode> Inputs { get; }
    
    public Dictionary<string, INandTreeNode> Outputs { get; }

    public ChipBlueprint(string name, Dictionary<string, NandPinNode> inputs, Dictionary<string, INandTreeNode> outputs)
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