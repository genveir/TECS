using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips.NandTree;

internal class ClockPin : INandTreeElement
{
    public INandTreeElement Clone(long cloneId)
    {
        return this;
    }

    public bool GetValue(long evaluationId)
    {
        //TODO: be more explicit about clocks
        return evaluationId % 2 == 0;
    }

    public INandTreeElement Fuse(long fuseId)
    {
        return this;
    }

    private bool _isValidated = false;
    public void Validate(List<ValidationError> errors, List<INandTreeElement> parentNodes, long validationRun)
    {
        _isValidated = true;
    }

    public bool IsValidatedInRun(long validationRun)
    {
        return _isValidated;
    }
}