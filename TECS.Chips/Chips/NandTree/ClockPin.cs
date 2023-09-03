using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

internal class ClockPin : INandTreeElement
{
    private ClockPin() { }
    
    public static readonly ClockPin Instance = new();
    
    public INandTreeElement Clone(long cloneId)
    {
        return this;
    }

    public bool GetValue(long clock)
    {
        return clock % 2 == 0;
    }

    public INandTreeElement FindFuseElement() => this;
    
    public INandTreeElement Fuse(long fuseId)
    {
        return this;
    }

    private bool _isValidated;
    public void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationRun)
    {
        _isValidated = true;
    }

    public bool IsValidatedInRun(long validationRun)
    {
        return _isValidated;
    }
}