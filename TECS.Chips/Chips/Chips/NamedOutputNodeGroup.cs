using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips;

public class NamedOutputNodeGroup : NamedNodeGroup<NamedOutputNodeGroup>
{
    internal override ReadOnlySpan<INandTreeElement> Nodes => _nodes;

    private INandTreeElement[] _nodes;

    public NamedOutputNodeGroup(NamedNodeGroupName name, BitSize bitSize) : base(name)
    {
        _nodes = new INandTreeElement[bitSize.Value];
        
        for (int n = 0; n < _nodes.Length; n++)
            _nodes[n] = new NandPinNode();
    }

    private NamedOutputNodeGroup(NamedNodeGroupName name, INandTreeElement[] nodes) : base(name)
    {
        _nodes = nodes;
    }

    public void Fuse(long fuseId)
    {
        for (int n = 0; n < _nodes.Length; n++)
            _nodes[n] = _nodes[n].Fuse(fuseId);
    }

    private long _validatedInRun = -1;
    public void Validate(List<ValidationError> errors, long validationRun)
    {
        if (validationRun == _validatedInRun)
        {
            errors.Add(new($"{this} is hit multiple times in validation run"));
            return;
        }

        _validatedInRun = validationRun;
        
        foreach (var node in _nodes)
        {
            node.Validate(errors, new(), validationRun);
        }
    }
    
    public override NamedOutputNodeGroup Clone(long cloneId)
    {
        var newNodes = new INandTreeElement[_nodes.Length];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = _nodes[n].Clone(cloneId);
        }

        return new(Name, newNodes);
    }
}