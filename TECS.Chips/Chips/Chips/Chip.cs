using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Chips;

public class Chip
{
    private static long _idCounter;
    private readonly long _id = _idCounter++;

    private readonly ChipName _name;
    private Dictionary<NamedNodeGroupName, InputNodeGroup> Inputs { get; }
    private Dictionary<NamedNodeGroupName, OutputNodeGroup> Outputs { get; }
    
    
    public Chip(ChipName chipName,
        Dictionary<NamedNodeGroupName, InputNodeGroup> inputs, 
        Dictionary<NamedNodeGroupName, OutputNodeGroup> outputs)
    {
        _name = chipName;
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

        Dictionary<NamedNodeGroupName, BitValue> inputResult = new();
        foreach (var input in Inputs)
        {
            var name = input.Key;
            var value = input.Value.GetValue(CachingCounter);
            
            inputResult.Add(name, value);
        }

        Dictionary<NamedNodeGroupName, BitValue> outputResult = new();
        foreach (var output in Outputs)
        {
            var name = output.Key;
            var value = output.Value.GetValue(CachingCounter);
            
            outputResult.Add(name, value);
        }
        
        return new(inputValues: inputResult, outputValues: outputResult);
    }

    public override string ToString() => $"Chip {_name} {_id}";
}