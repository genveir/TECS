using System.Collections.Generic;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class Chip
{
    public Dictionary<string, NandPinNode> Inputs { get; }

    public INandTreeNode Output { get; }
    
    public Chip(Dictionary<string, NandPinNode> inputs, INandTreeNode output)
    {
        Inputs = inputs;
        Output = output;
    }

    public bool[] Evaluate() => Output.Value;
}