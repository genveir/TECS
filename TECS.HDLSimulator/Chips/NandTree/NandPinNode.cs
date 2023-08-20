using System;

namespace TECS.HDLSimulator.Chips.NandTree;

public class NandPinNode : INandTreeNode
{
    public INandTreeNode? Parent { get; set; }

    private bool[] _value = Array.Empty<bool>();
    public bool[] Value
    {
        get => Parent == null ? _value : Parent.Value;
        set => _value = value;
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
}