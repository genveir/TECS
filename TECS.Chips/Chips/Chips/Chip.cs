using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    public Dictionary<NamedNodeGroupName, InputNodeGroup> Inputs { get; }
    public Dictionary<NamedNodeGroupName, OutputNodeGroup> Outputs { get; }
    
    public Chip(Dictionary<NamedNodeGroupName, InputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }
}