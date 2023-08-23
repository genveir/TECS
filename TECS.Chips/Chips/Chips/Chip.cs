using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    public Dictionary<string, NamedNodeGroup> Inputs { get; }
    public Dictionary<string, NamedNodeGroup> Outputs { get; }
    
    public Chip(Dictionary<string, NamedNodeGroup> inputs, Dictionary<string, NamedNodeGroup> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }
    
    public bool[] Evaluate(string name) => Outputs[name].Nodes
        .Select(p => p.Value)
        .ToArray();
}