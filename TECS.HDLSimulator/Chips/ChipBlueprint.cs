using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprint
{
    public string Name { get; }
    public Dictionary<string, NandPinNode> Inputs { get; }
    
    public string OutputName { get; }
    public INandTreeNode Output { get; private set; }

    private bool _isPrefused;

    public ChipBlueprint(string name, Dictionary<string, NandPinNode> inputs, string outputName, INandTreeNode output, bool fuse = true)
    {
        Inputs = inputs;
        OutputName = outputName;
        Output = output;

        if (fuse) Fuse();
    }

    public void Fuse()
    {
        _isPrefused = true;

        Output = Output.Fuse(_cloneCounter++);
    }

    private static long _cloneCounter;
    public Chip Fabricate()
    {
        var output = Output.Clone(_cloneCounter);

        var inputs = Inputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ClonePin(_cloneCounter));

        if (!_isPrefused) 
            output = output.Fuse(_cloneCounter);
        
        _cloneCounter++;

        return new(inputs, output);
    }
}