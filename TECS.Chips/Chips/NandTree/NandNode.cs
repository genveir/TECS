using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

internal class NandNode : INandTreeElement
{
    private static long _idCounter;
    private readonly long _id = _idCounter++;

    private INandTreeElement _a = new NandPinNode();
    private INandTreeElement _b = new NandPinNode();

    
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

    public bool GetValue() => !(_a.GetValue() && _b.GetValue()); 
    
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

    public override string ToString()
    {
        return $"NandNode {_id}";
    }
}