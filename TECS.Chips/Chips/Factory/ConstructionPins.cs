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
        if (name == "true")
        {
            if (bitSize == 1) return TrueGroup;
            if (bitSize == 16) return TrueGroup16;
        }

        if (name == "false")
        {
            if (bitSize == 1) return FalseGroup;
            if (bitSize == 16) return FalseGroup16;
        }
        
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

    private static NamedNodeGroup? _true16;
    private static NamedNodeGroup TrueGroup16
    {
        get
        {
            if (_true16 == null)
            {
                _true16 = new NamedNodeGroup("true", 16);
                for (int n = 0; n < 16; n++) _true16.Nodes[n] = ConstantPin.True;
            }

            return _true16;
        }
    }

    private static readonly NamedNodeGroup FalseGroup = new("false", 1)
    {
        Nodes =
        {
            [0] = ConstantPin.False
        }
    };
    
    private static NamedNodeGroup? _false16;
    private static NamedNodeGroup FalseGroup16
    {
        get
        {
            if (_false16 == null)
            {
                _false16 = new NamedNodeGroup("true", 16);
                for (int n = 0; n < 16; n++) _false16.Nodes[n] = ConstantPin.False;
            }

            return _false16;
        }
    }
}