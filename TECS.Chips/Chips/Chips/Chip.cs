using System.Collections.Generic;
using TECS.DataIntermediates.Names;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    public Dictionary<NamedNodeGroupName, NamedInputNodeGroup> Inputs { get; }
    public Dictionary<NamedNodeGroupName, NamedOutputNodeGroup> Outputs { get; }
    
    public Chip(Dictionary<NamedNodeGroupName, NamedInputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, NamedOutputNodeGroup> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }
}