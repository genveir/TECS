using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

internal class ConstantPin : INandTreeElement
{
    private readonly bool _value;
    
    private ConstantPin(bool value)
    {
        _value = value;
    }

    public static readonly ConstantPin True = new(true);
    public static readonly ConstantPin False = new(false);

    public INandTreeElement Clone(long cloneId)
    {
        return this;
    }

    public bool GetValue() => _value;

    public INandTreeElement Fuse(long fuseId)
    {
        return this;
    }

    private long _validationId = -1;
    public void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationRun)
    {
        _validationId = validationRun;
    }

    public bool IsValidatedInRun(long validationRun)
    {
        return _validationId == validationRun;
    }
}