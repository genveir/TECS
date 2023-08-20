using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprint
{
    public string Name { get; }
    public Dictionary<string, NandPinNode> Inputs { get; }
    
    public Dictionary<string, INandTreeNode> Outputs { get; }

    private bool _isPrefused;

    public ChipBlueprint(string name, Dictionary<string, NandPinNode> inputs, Dictionary<string, INandTreeNode> outputs, bool fuse = true)
    {
        Inputs = inputs;
        Outputs = outputs;

        if (fuse) Fuse();
    }

    public void Fuse()
    {
        _isPrefused = true;

        FuseOutputs(Outputs);
        
        _cloneCounter++;
    }

    private static long _cloneCounter;
    public Chip Fabricate()
    {
        var cloned = Clone();

        return new(cloned.Inputs, cloned.Outputs);
    }

    public ChipBlueprint Clone()
    {
        var outputs = Outputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Clone(_cloneCounter));

        var inputs = Inputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ClonePin(_cloneCounter));

        if (!_isPrefused)
        {
            FuseOutputs(outputs);
        }

        _cloneCounter++;

        return new(Name, inputs, outputs, _isPrefused);
    }

    private static void FuseOutputs(Dictionary<string, INandTreeNode> outputs)
    {
        foreach (var output in outputs)
        {
            var name = output.Key;
            var fusedOutput = output.Value.Fuse(_cloneCounter);

            outputs[name] = fusedOutput;
        }
    }
}