using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprint
{
    public Dictionary<string, NandPinNode> Inputs { get; }
    
    public string OutputName { get; }
    public INandTreeNode Output { get; private set; }

    public ChipBlueprint(Dictionary<string, NandPinNode> inputs, string outputName, INandTreeNode output)
    {
        Inputs = inputs;
        OutputName = outputName;
        Output = output;
    }
    
    private static long _cloneCounter;
    public Chip Fabricate()
    {
        var output = Output.Clone(_cloneCounter);

        var inputs = Inputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ClonePin(_cloneCounter));

        output = output.Fuse(_cloneCounter);
        
        _cloneCounter++;

        return new(inputs, output);
    }
}