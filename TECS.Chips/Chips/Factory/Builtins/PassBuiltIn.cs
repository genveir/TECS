using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Factory.Builtins;

internal static class PassBuiltIn
{
    private static StoredBlueprint? _blueprint;
    
    public static StoredBlueprint GetBlueprint()
    {
        if (_blueprint == null)
        {
            var input = new InputNodeGroup(new(new("in"), new BitSize(1)));
            var outPin = new PinBoard(new("out"), new BitSize(1));
            outPin.Nodes[0].Parent = input.Nodes[0];
            var output = new OutputNodeGroup(outPin);

            var inputs = new Dictionary<NamedNodeGroupName, InputNodeGroup>
            {
                { new("in"), input }
            };
            var outputs = new Dictionary<NamedNodeGroupName, OutputNodeGroup>
            {
                { new("out"), output }
            };

            _blueprint = new(new("Pass"), inputs, outputs);
        }

        return _blueprint;
    }
}