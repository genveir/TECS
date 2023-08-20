using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips;

public class Chip
{
    public Dictionary<string, NandPinNode> Inputs { get; }
    
    public string OutputName { get; }
    public INandTreeNode Output { get; private set; }

    public Chip(Dictionary<string, NandPinNode> inputs, string outputName, INandTreeNode output)
    {
        Inputs = inputs;
        OutputName = outputName;
        Output = output;
    }

    public void Fuse(int runId = 1)
    {
        Output = Output.Fuse(runId);
    }
    
    public bool[] Evaluate() => Output.Value;

    public INandTreeNode GetTree() => Output;
}