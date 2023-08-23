using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

public class NandNode : INandTreeElement
{
    private static long _idCounter;
    private readonly long _id = _idCounter++;

    private INandTreeElement _a = new NandPinNode();
    private INandTreeElement _b = new NandPinNode();

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

    private long _setAsInputInRun = -1;

    public void SetAsInputForValidation(List<ValidationError> errors, long validationRun)
    {
        if (_setAsInputInRun == validationRun) return;
        _setAsInputInRun = validationRun;

        errors.Add(new($"{this} is set as input"));
    }

    private long _validatedInRun = -1;

    public void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationId)
    {
        if (_validatedInRun == validationId) return;
        _validatedInRun = validationId;

        if (parentNodes.Contains(this))
        {
            errors.Add(new($"{this} has cyclical connection"));
            return;
        }

        var newParents = new List<INandTreeElement>(parentNodes) { this };

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_a == null) errors.Add(new($"{this} missing a-input"));
        else _a.Validate(errors, newParents, validationId);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_b == null) errors.Add(new($"{this} missing b-input"));
        else _b.Validate(errors, newParents, validationId);
    }

    public bool IsValidatedInRun(long validationId) => _validatedInRun == validationId;

    public bool Value { 
        get => !(_a.Value && _b.Value);
        set {} // TODO: fix clunky types
    }

private long _cloneId = -1;
    private INandTreeElement? _cloneResult;
    public INandTreeElement Clone(long cloneId)
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
    public INandTreeElement Fuse(long fuseId)
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