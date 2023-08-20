using System;

namespace TECS.HDLSimulator.Chips;

public interface INandTreeNode
{
    bool[] Value { get; }

    INandTreeNode Fuse(int runId);

    (int pins, int nands) CountNodes(int runId);
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

    private int _runId = -1;

    public INandTreeNode Fuse(int runId)
    {
        if (_runId == runId) return this;
        _runId = runId;
        
        return Parent == null ? this : Parent.Fuse(runId);
    }

    public (int pins, int nands) CountNodes(int runId)
    {
        if (_runId == runId) return (0, 0);
        _runId = runId;
        
        if (Parent == null) return (1, 0);
        
        var (parentPins, parentNands) = Parent.CountNodes(runId);
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

    private int _runId = -1;
    public INandTreeNode Fuse(int runId)
    {
        if (_runId == runId) return this;
        _runId = runId;
        
        _a = _a.Fuse(runId);
        _b = _b.Fuse(runId);

        return this;
    }
    
    public (int pins, int nands) CountNodes(int runId)
    {
        if (runId == _runId) return (0, 0);
        _runId = runId;
        
        var (aPins, aNands) = _a.CountNodes(runId);
        var (bPins, bNands) = _b.CountNodes(runId);

        return (aPins + bPins, aNands + bNands + 1);
    }
}