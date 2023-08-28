using System.Collections.Generic;
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
}