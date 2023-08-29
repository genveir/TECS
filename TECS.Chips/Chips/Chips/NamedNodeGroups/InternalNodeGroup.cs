using System;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

public class InternalNodeGroup : NamedNodeGroup<InternalNodeGroup>
{
    internal override ReadOnlySpan<INandTreeElement> Nodes => _nodes;
    
    private readonly NandPinNode[] _nodes;
    private InternalNodeGroup(NamedNodeGroupName name, NandPinNode[] nodes) : base(name)
    {
        _nodes = nodes;
    }

    internal InternalNodeGroup(PinBoard pinBoard) : this(pinBoard.Name, pinBoard.Nodes) { }

    internal override InternalNodeGroup Clone(long cloneId)
    {
        var newNodes = new NandPinNode[_nodes.Length];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = _nodes[n].ClonePin(cloneId);
        }

        return new(Name, newNodes);
    }
}