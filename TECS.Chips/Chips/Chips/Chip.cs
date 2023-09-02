using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    protected long EvaluationId;
    
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

    public EvaluationResult Evaluate()
    {
        EvaluationId++;

        return new(
            inputValues: Inputs.ToDictionary(inp => inp.Key, inp => inp.Value.GetValue(EvaluationId)),
            outputValues: Outputs.ToDictionary(op => op.Key, op => op.Value.GetValue(EvaluationId)));
    }
}