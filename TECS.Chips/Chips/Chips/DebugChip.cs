using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class DebugChip : Chip
{
    public ChipName Name { get; }
    
    public Dictionary<NamedNodeGroupName, OutputNodeGroup> Internals { get; }

    public DebugChip(
        ChipName name,
        Dictionary<NamedNodeGroupName, InputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs,
        Dictionary<NamedNodeGroupName, OutputNodeGroup> internals) : base(inputs, outputs)
    {
        Name = name;
        Internals = internals;
    }

    public override string ToString()
    {
        return $"DebugChip {Name}";
    }
}