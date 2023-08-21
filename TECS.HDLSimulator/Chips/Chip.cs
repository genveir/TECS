using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class Chip
{
    public Dictionary<string, NandPinNode> Inputs { get; }
    public Dictionary<string, INandTreeNode> Outputs { get; }


    public Chip(Dictionary<string, NandPinNode> inputs, Dictionary<string, INandTreeNode> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }

    public Dictionary<string, bool> EvaluateAll() => Outputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Value);

    public bool Evaluate(string name) => Outputs[name].Value;
}