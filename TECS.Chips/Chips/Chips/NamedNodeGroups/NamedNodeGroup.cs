using System;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

public abstract class NamedNodeGroup<TImplementingType>
{
    public NamedNodeGroupName Name { get; }
    
    internal abstract ReadOnlySpan<INandTreeElement> Nodes { get; }

    internal NamedNodeGroup(NamedNodeGroupName name)
    {
        Name = name;
    }
    
    public BitValue GetValue(long evaluationId)
    {
        var value = new bool[Nodes.Length];
        for (int n = 0; n < Nodes.Length; n++)
        {
            value[n] = Nodes[n].GetValue(evaluationId);
        }

        return new(value);
    }

    // cloneId is used to not re-clone the inputs, but use the ones from the cloned tree
    internal abstract TImplementingType Clone(long cloneId);

    public override string ToString()
    {
        return $"NamedPinGroup {Name}[{Nodes.Length}]";
    }
}