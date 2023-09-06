using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory.Builtins;

internal static class NandBuiltIn
{
    private static StoredBlueprint? _blueprint;
    
    public static StoredBlueprint GetBlueprint()
    {
        if (_blueprint == null)
        {
            var aInput = new InputNodeGroup(new(new("a"), new BitSize(1)));
            var bInput = new InputNodeGroup(new(new("b"), new BitSize(1)));

            var nandNode = new NandNode(aInput.Nodes[0], bInput.Nodes[0]);
            var outPin = new NandPinNode()
            {
                Parent = nandNode
            };
            var pinBoard = new PinBoard(new("out"), new BitSize(1));
            pinBoard.Nodes[0] = outPin;

            var outputGroup = new OutputNodeGroup(pinBoard);

            var inputs = new Dictionary<NamedNodeGroupName, InputNodeGroup>
            {
                { new("a"), aInput },
                { new("b"), bInput }
            };
            var outputs = new Dictionary<NamedNodeGroupName, OutputNodeGroup>
            {
                { new("out"), outputGroup }
            };
            
            _blueprint = new(new("Nand"), inputs, outputs);
        }

        return _blueprint;
    }
}