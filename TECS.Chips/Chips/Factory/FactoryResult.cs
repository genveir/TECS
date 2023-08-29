using System.Collections.Generic;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

public class FactoryResult<TResult>
{
    public bool Success { get; }
    public TResult? Result { get; }
    public List<ValidationError> Errors { get; }

    private FactoryResult(bool success, TResult? result, List<ValidationError> errors)
    {
        Success = success;
        Result = result;
        Errors = errors;
    }

    public static FactoryResult<TResult> Succeed(TResult result)
    {
        return new(true, result, new List<ValidationError>());
    }

    public static FactoryResult<TResult> Fail(List<ValidationError> errors)
    {
        return new(false, default, errors);
    }
}