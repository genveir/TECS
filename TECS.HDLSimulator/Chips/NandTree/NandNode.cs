using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

public class NandNode : INandTreeNode
{
    private static long _idCounter;
    private readonly long _id = _idCounter++;
    
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

    private long _validatedInRun = -1;
    public void Validate(List<ValidationError> errors, NandPinNode[] inputs, List<INandTreeNode> parentNodes, long validationId)
    {
        if (_validatedInRun == validationId) return;
        _validatedInRun = validationId;
        
        if (parentNodes.Contains(this))
        {
            errors.Add(new($"Nand node {_id} has cyclical connection"));
            return;
        }

        var newParents = new List<INandTreeNode>(parentNodes);
        newParents.Add(this);

        if (_a == null) errors.Add(new($"Nand node {_id} missing a-input"));
        else _a.Validate(errors, inputs, newParents, validationId);
        
        if (_b == null) errors.Add(new($"Nand node {_id} missing b-input"));
        else _b.Validate(errors, inputs, newParents, validationId);
    }
    
    public bool Value => !(_a.Value && _b.Value);

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

    public override string ToString()
    {
        return $"NandNode {_id}";
    }
}