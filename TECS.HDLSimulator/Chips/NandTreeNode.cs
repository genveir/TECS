using System;

namespace TECS.HDLSimulator.Chips;

public interface INandTreeNode
{
    bool[] Value { get; }

    INandTreeNode Fuse(int fuseId);

    (int pins, int nands) CountNodes(int countId);
}

public class NandPinNode : INandTreeNode
{
    public INandTreeNode? Parent { get; set; }

    private bool[] _value = Array.Empty<bool>();
    public bool[] Value
    {
        get => Parent == null ? _value : Parent.Value;
        set => _value = value;
    }

    private int _fuseId = -1;
    private INandTreeNode _fuseResult;
    public INandTreeNode Fuse(int fuseId)
    {
        if (_fuseId == fuseId) return _fuseResult;
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

public class NandNode : INandTreeNode
{
    private INandTreeNode _a = new NandPinNode();
    private INandTreeNode _b = new NandPinNode();

    public bool TryGetInputPins(out (NandPinNode a, NandPinNode b) result)
    {
        if (_a is NandPinNode aNode && _b is NandPinNode bNode)
        {
            result = (aNode, bNode);
            return true;
        }

        result = default;
        return false;
    }
    
    public bool[] Value => new[] { !(_a.Value[0] && _b.Value[0]) };

    private int _fuseId = -1;
    public INandTreeNode Fuse(int fuseId)
    {
        if (_fuseId == fuseId) return this;
        _fuseId = fuseId;
        
        _a = _a.Fuse(fuseId);
        _b = _b.Fuse(fuseId);

        return this;
    }

    private int _countId = -1;
    public (int pins, int nands) CountNodes(int countId)
    {
        if (countId == _countId) return (0, 0);
        _countId = countId;
        
        var (aPins, aNands) = _a.CountNodes(countId);
        var (bPins, bNands) = _b.CountNodes(countId);

        return (aPins + bPins, aNands + bNands + 1);
    }
}