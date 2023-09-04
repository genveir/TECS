using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

public class OutputNodeGroup : NamedNodeGroup<OutputNodeGroup>
{
    internal override ReadOnlySpan<INandTreeElement> Nodes => _nodes;
    protected override BitSize Size { get; }

    private readonly INandTreeElement[] _nodes;

    private OutputNodeGroup(NamedNodeGroupName name, INandTreeElement[] nodes) : base(name)
    {
        _nodes = nodes;
        Size = new(_nodes.Length);
    }
    
    internal OutputNodeGroup(PinBoard pinBoard) : this(pinBoard.Name, pinBoard.CopyNodesToElements()) { }

    internal static OutputNodeGroup NandGroup(NandNode node) => new(new("out"), new[] { node });
    
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
    
    internal override OutputNodeGroup Clone(long cloneId)
    {
        var newNodes = new INandTreeElement[_nodes.Length];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = _nodes[n].Clone(cloneId);
        }

        return new(Name, newNodes);
    }
}