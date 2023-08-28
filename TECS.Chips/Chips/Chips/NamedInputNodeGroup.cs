using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips;

public class NamedInputNodeGroup : NamedNodeGroup<NamedInputNodeGroup>
{
    internal override ReadOnlySpan<INandTreeElement> Nodes => _nodes;
    
    private readonly ISettableElement[] _nodes;
    
    public NamedInputNodeGroup(NamedNodeGroupName name, BitSize bitSize) : base(name)
    {
        _nodes = new ISettableElement[bitSize.Value];

        for (int n = 0; n < _nodes.Length; n++)
            _nodes[n] = new NandPinNode();
    }

    private NamedInputNodeGroup(NamedNodeGroupName name, ISettableElement[] nodes) : base(name)
    {
        _nodes = nodes;
    }
    
    public void SetValue(BitValue value)
    {
        if (value.Size.Value == _nodes.Length)
        {
            for (int n = 0; n < _nodes.Length; n++)
            {
                _nodes[n].SetValue(value.Value[n]);
            }
        }
    }
    
    public void SetAsInputForValidation(List<ValidationError> errors, long validationRun)
    {
        foreach (var node in _nodes)
        {
            node.SetAsInputForValidation(errors, validationRun);
        }
    }

    public override NamedInputNodeGroup Clone(long cloneId)
    {
        var newNodes = new ISettableElement[_nodes.Length];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = _nodes[n].CloneAsSettable(cloneId);
        }

        return new(Name, newNodes);
    }
}