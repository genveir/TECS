using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.Chips.NandTree;

public class NandPinNode : INandTreeNode
{
    private static long _idCounter;
    public readonly long Id = _idCounter++;
    
    public INandTreeNode? Parent { get; set; }

    private bool _value;
    public bool Value
    {
        get => Parent?.Value ?? _value;
        set => _value = value;
    }

    public long ValidatedInRun = -1;
    public void Validate(List<ValidationError> errors, NandPinNode[] inputs, List<INandTreeNode> parentNodes, long validationId)
    {
        if (ValidatedInRun == validationId) return;
        ValidatedInRun = validationId;
        
        if (parentNodes.Contains(this))
        {
            errors.Add(new($"Pin {Id} has cyclical connection"));
            return;
        }
        var newParents = new List<INandTreeNode>(parentNodes);
        newParents.Add(this);

        if (!inputs.Any(i => i == this) && Parent == null)
            errors.Add(new($"Non input pin {Id} has no parent"));

        Parent?.Validate(errors, inputs, newParents, validationId);
    }

    private long _cloneId = -1;
    private NandPinNode? _cloneResult;
    public INandTreeNode Clone(long cloneId) => ClonePin(cloneId);

    public NandPinNode ClonePin(long cloneId)
    {
        if (_cloneId == cloneId) return _cloneResult!;
        _cloneId = cloneId;
        
        var newPin = new NandPinNode();

        if (Parent != null) newPin.Parent = Parent.Clone(cloneId);

        _cloneResult = newPin;
        return _cloneResult;
    }
    
    private long _fuseId = -1;
    private INandTreeNode? _fuseResult;
    public INandTreeNode Fuse(long fuseId)
    {
        if (_fuseId == fuseId) return _fuseResult!;
        _fuseId = fuseId;
        
        _fuseResult = Parent == null ? this : Parent.Fuse(fuseId);
        return _fuseResult;
    }
    
    private int _countId = -1;
    public (int pins, int nands) CountNodes(int countId)
    {
        if (_countId == countId) return (0, 0);
        _countId = countId;
        
        if (Parent == null) return (1, 0);
        
        var (parentPins, parentNands) = Parent.CountNodes(countId);
        return (parentPins + 1, parentNands);
    }

    public override string ToString()
    {
        return $"NandPinNode {Id}";
    }
}