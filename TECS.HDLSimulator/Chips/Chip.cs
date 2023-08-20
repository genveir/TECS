using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips;

public class Chip
{
    public Dictionary<string, NandPinNode> Inputs { get; }

    public INandTreeNode Output;
    
    public Chip(Dictionary<string, NandPinNode> inputs, INandTreeNode output)
    {
        Inputs = inputs;
        Output = output;
    }

    public bool[] Evaluate() => Output.Value;
}