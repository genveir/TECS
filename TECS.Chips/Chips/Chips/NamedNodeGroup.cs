using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips;

public abstract class NamedNodeGroup<TImplementingType>
{
    protected NamedNodeGroupName Name { get; }
    
    internal abstract ReadOnlySpan<INandTreeElement> Nodes { get; }

    internal NamedNodeGroup(NamedNodeGroupName name)
    {
        Name = name;
    }
    
    public BitValue GetValue()
    {
        var value = new bool[Nodes.Length];
        for (int n = 0; n < Nodes.Length; n++)
        {
            value[n] = Nodes[n].GetValue();
        }

        return new(value);
    }

    // cloneId is used to not re-clone the inputs, but use the ones from the cloned tree
    public abstract TImplementingType Clone(long cloneId);

    public void IsValidatedInRun(List<ValidationError> errors, long validationRun)
    {
        foreach (var node in Nodes)
        {
            if (!node.IsValidatedInRun(validationRun))
                errors.Add(new($"{node} is an unconnected input"));
        }
    }

    public override string ToString()
    {
        return $"NamedPinGroup {Name}[{Nodes.Length}]";
    }
}