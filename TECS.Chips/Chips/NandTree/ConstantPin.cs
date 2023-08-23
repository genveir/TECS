using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

public class ConstantPin : INandTreeElement
{
    private ConstantPin(bool value)
    {
        Value = value;
    }

    public static readonly ConstantPin True = new(true);
    public static readonly ConstantPin False = new(false);

    public INandTreeElement Clone(long cloneId)
    {
        return this;
    }

    public bool Value
    {
        get; 
        set; // TODO: fix wonky contract
    }
    public INandTreeElement Fuse(long fuseId)
    {
        return this;
    }

    private int _countId = 1;
    public (int pins, int nands) CountNodes(int countId)
    {
        if (_countId == countId) return (0, 0);

        return (1, 0);
    }

    private long _validationId = -1;
    public void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationRun)
    {
        _validationId = validationRun;
    }

    public void SetAsInputForValidation(List<ValidationError> errors, long validationRun)
    {
        
    }

    public bool IsValidatedInRun(long validationRun)
    {
        return _validationId == validationRun;
    }
}