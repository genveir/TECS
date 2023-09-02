using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.HDLSimulator.Chips.Chips;

public class EvaluationResult
{
    public readonly Dictionary<NamedNodeGroupName, BitValue> InputValues;
    public readonly Dictionary<NamedNodeGroupName, BitValue> OutputValues;

    public EvaluationResult(
        Dictionary<NamedNodeGroupName, BitValue> inputValues,
        Dictionary<NamedNodeGroupName, BitValue> outputValues)
    {
        InputValues = inputValues;
        OutputValues = outputValues;
    }
}

public class DebugEvaluationResult : EvaluationResult
{
    public Dictionary<NamedNodeGroupName, BitValue> InternalValues;

    public DebugEvaluationResult(
        Dictionary<NamedNodeGroupName, BitValue> inputValues,
        Dictionary<NamedNodeGroupName, BitValue> outputValues,
        Dictionary<NamedNodeGroupName, BitValue> internalValues) : base(inputValues, outputValues)
    {
        InternalValues = internalValues;
    }
}