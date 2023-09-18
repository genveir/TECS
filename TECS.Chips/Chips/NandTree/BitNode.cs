using System.Collections.Generic;
using TECS.HDLSimulator.Chips.Chips;

namespace TECS.HDLSimulator.Chips.NandTree;

internal class BitNode : INandTreeElement
{
    private static long _idCounter;
    private readonly long _id = _idCounter++;
    
    private INandTreeElement _input;
    private INandTreeElement _load;

    public BitNode(INandTreeElement input, INandTreeElement load)
    {
        _input = input;
        _load = load;

        _cloneResult = this;
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
        if (_input == null) errors.Add(new($"{this} missing input"));
        else _input.Validate(errors, newParents, validationId);

        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (_load == null) errors.Add(new($"{this} missing load"));
        else _load.Validate(errors, newParents, validationId);
    }
    
    
    private long _cloneId = -1;
    private INandTreeElement _cloneResult;
    public INandTreeElement Clone(long cloneId)
    {
        if (_cloneId != cloneId)
        {
            _cloneId = cloneId;

            var newNode = new BitNode(
                input: _input,
                load: _load);

            _cloneResult = newNode;

            newNode._input = _input.Clone(cloneId);
            newNode._load = _load.Clone(cloneId);
        }

        return _cloneResult;
    }

    public bool IsValidatedInRun(long validationId) => _validatedInRun == validationId;

    private bool _value;
    
    private long _evaluationId = -1;
    private bool _cachedValue;
    public bool GetValue(long cachingCounter)
    {
        if (_evaluationId != cachingCounter)
        {
            _evaluationId = cachingCounter;
            _cachedValue = _value;
            
            if (_load.GetValue(cachingCounter) && Clock.Instance.Potential)
                _value = _input.GetValue(cachingCounter);
        }

        return _cachedValue;
    }

    private long _fuseId = -1;

    public INandTreeElement FindFuseElement() => this;
    public INandTreeElement Fuse(long fuseId)
    {
        if (_fuseId != fuseId)
        {
            _fuseId = fuseId;
            
            _input = _input.Fuse(fuseId);
            _load = _load.Fuse(fuseId);
        }

        return this;
    }

    public override string ToString()
    {
        return $"BitNode {_id}";
    }
}