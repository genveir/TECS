namespace TECS.HDLSimulator.Chips.NandTree;

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

    private long _cloneId = -1;
    private INandTreeNode? _cloneResult;
    public INandTreeNode Clone(long cloneId)
    {
        if (_cloneId == cloneId) return _cloneResult!;
        _cloneId = cloneId;
        
        var newNand = new NandNode
        {
            _a = _a.Clone(cloneId),
            _b = _b.Clone(cloneId)
        };

        _cloneResult = newNand;
        return _cloneResult;
    }

    private long _fuseId = -1;
    public INandTreeNode Fuse(long fuseId)
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