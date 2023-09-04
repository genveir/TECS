using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    private Dictionary<NamedNodeGroupName, InputNodeGroup> Inputs { get; }
    private Dictionary<NamedNodeGroupName, OutputNodeGroup> Outputs { get; }
    
    public Chip(Dictionary<NamedNodeGroupName, InputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs)
    {
        Inputs = inputs;
        Outputs = outputs;
    }

    public IEnumerable<NamedNodeGroupName> InputNames => Inputs.Keys.ToArray();
    public IEnumerable<NamedNodeGroupName> OutputNames => Outputs.Keys.ToArray();
    
    public void SetInput(NamedNodeGroupName inputGroup, BitValue bitValue) => 
        Inputs[inputGroup].SetValue(bitValue);

    public void IncrementClock()
    {
        Clock.Instance.Increment();
    }

    protected long CachingCounter = -1;
    public EvaluationResult Evaluate()
    {
        CachingCounter++;
        
        return new(
            inputValues: Inputs.ToDictionary(inp => inp.Key, inp => inp.Value.GetValue(CachingCounter)),
            outputValues: Outputs.ToDictionary(op => op.Key, op => op.Value.GetValue(CachingCounter)));
    }
}