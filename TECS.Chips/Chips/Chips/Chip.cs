using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    public Dictionary<NamedNodeGroupName, NamedNodeGroup> Inputs { get; }
    public Dictionary<NamedNodeGroupName, NamedNodeGroup> Outputs { get; }
    
    public Chip(Dictionary<NamedNodeGroupName, NamedNodeGroup> inputs, Dictionary<NamedNodeGroupName, NamedNodeGroup> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }
    
    public bool[] Evaluate(NamedNodeGroupName name) => Outputs[name].Nodes
        .Select(p => p.Value)
        .ToArray();
}