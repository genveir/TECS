using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

public class NandPinNode : INandTreeElement
{
    private static long _idCounter;
    private readonly long _id = _idCounter++;
    
    public INandTreeElement? Parent { get; set; }

    private bool _value;
    public bool Value
    {
        get => Parent?.Value ?? _value;
        set => _value = value;
    }
    
    private long _isInputForRun = -1;
    public void SetAsInputForValidation(List<ValidationError> errors, long validationId)
    {
        if (Parent != null)
            errors.Add(new($"{this} is an input but has parent {Parent}"));

        _isInputForRun = validationId;
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

        var isInput = _isInputForRun == validationId;
        
        if (!isInput && Parent == null)
            errors.Add(new($"Non input {this} has no parent"));

        Parent?.Validate(errors, newParents, validationId);
    }

    public bool IsValidatedInRun(long validationId) => _validatedInRun == validationId;

    private long _cloneId = -1;
    private NandPinNode? _cloneResult;
    public INandTreeElement Clone(long cloneId) => ClonePin(cloneId);

    private NandPinNode ClonePin(long cloneId)
    {
        if (_cloneId == cloneId) return _cloneResult!;
        _cloneId = cloneId;
        
        var newPin = new NandPinNode();

        if (Parent != null) newPin.Parent = Parent.Clone(cloneId);

        _cloneResult = newPin;
        return _cloneResult;
    }
    
    private long _fuseId = -1;
    private INandTreeElement? _fuseResult;
    public INandTreeElement Fuse(long fuseId)
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
        return $"NandPinNode {_id}";
    }
}