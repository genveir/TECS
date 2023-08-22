using System;
using System.Collections.Generic;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

internal class ConstructionPins
{
    public Dictionary<string, NamedNodeGroup> InputPins { get; }
    public Dictionary<string, NamedNodeGroup> OutputPins { get; }
    public Dictionary<string, NamedNodeGroup> InternalPins { get; }
        
    public ConstructionPins(Dictionary<string, NamedNodeGroup> inputPins, Dictionary<string, NamedNodeGroup> outputPins, Dictionary<string, NamedNodeGroup> internalPins)
    {
        InputPins = inputPins;
        OutputPins = outputPins;
        InternalPins = internalPins;
    }

    public NamedNodeGroup? GetNodeGroup(string name)
    {
        if (InputPins.TryGetValue(name, out NamedNodeGroup? namedNodeGroup)) return namedNodeGroup;
        if (OutputPins.TryGetValue(name, out namedNodeGroup)) return namedNodeGroup;
        if (InternalPins.TryGetValue(name, out namedNodeGroup)) return namedNodeGroup;

        return namedNodeGroup;
    }

    public NamedNodeGroup CreateInternalNodeGroup(string name, int bitSize)
    {
        if (name == "true") return TrueGroup;
        if (name == "false") return FalseGroup;
        
        var newGroup = new NamedNodeGroup(name, bitSize);
        InternalPins.Add(name, newGroup);

        return newGroup;
    }

    private static readonly NamedNodeGroup TrueGroup = new("true", 1)
    {
        Nodes =
        {
            [0] = ConstantPin.True
        }
    };

    private static readonly NamedNodeGroup FalseGroup = new("false", 1)
    {
        Nodes =
        {
            [0] = ConstantPin.False
        }
    };
}