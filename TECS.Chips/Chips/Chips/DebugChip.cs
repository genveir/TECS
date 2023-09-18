using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class DebugChip : Chip
{
    private Dictionary<NamedNodeGroupName, OutputNodeGroup> Internals { get; }

    public DebugChip(
        ChipName name,
        Dictionary<NamedNodeGroupName, InputNodeGroup> inputs,
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs,
        Dictionary<NamedNodeGroupName, OutputNodeGroup> internals) : base(name, inputs, outputs)
    {
        Internals = internals;
    }

    public IEnumerable<NamedNodeGroupName> InternalNames => Internals.Keys.ToArray();

    public DebugEvaluationResult DebugEvaluate()
    {
        var baseEval = Evaluate();

        return new(
            inputValues: baseEval.InputValues,
            outputValues: baseEval.OutputValues,
            internalValues: Internals.ToDictionary(ig => ig.Key, ig => ig.Value.GetValue(CachingCounter)));
    }
}