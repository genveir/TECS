using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class DebugChip : Chip
{
    public ChipName Name { get; }

    protected Dictionary<NamedNodeGroupName, OutputNodeGroup> Internals { get; }

    public DebugChip(
        ChipName name,
        Dictionary<NamedNodeGroupName, InputNodeGroup> inputs,
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs,
        Dictionary<NamedNodeGroupName, OutputNodeGroup> internals) : base(inputs, outputs)
    {
        Name = name;
        Internals = internals;
    }

    public IEnumerable<NamedNodeGroupName> InternalNames => Internals.Keys.ToArray();

    public BitValue GetInternal(NamedNodeGroupName name) =>
        Internals[name].GetValue();

    public override string ToString()
    {
        return $"DebugChip {Name}";
    }
}