using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

public class InputNodeGroup : NamedNodeGroup<InputNodeGroup>
{
    internal override ReadOnlySpan<INandTreeElement> Nodes => _nodes;
    
    private readonly NandPinNode[] _nodes;
    
    private InputNodeGroup(NamedNodeGroupName name, NandPinNode[] nodes) : base(name)
    {
        _nodes = nodes;
    }
    
    internal InputNodeGroup(PinBoard pinBoard) : this(pinBoard.Name, pinBoard.Nodes) { }
    
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
    
    public void IsValidatedInRun(List<ValidationError> errors, long validationRun)
    {
        foreach (var node in Nodes)
        {
            if (!node.IsValidatedInRun(validationRun))
                errors.Add(new($"{node} is an unconnected input"));
        }
    }

    internal override InputNodeGroup Clone(long cloneId)
    {
        var newNodes = new NandPinNode[_nodes.Length];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = _nodes[n].ClonePin(cloneId);
        }

        return new(Name, newNodes);
    }
}