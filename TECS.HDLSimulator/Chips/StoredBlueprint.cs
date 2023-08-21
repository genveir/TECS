using System;
using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class StoredBlueprint
{
    public string Name { get; }
    private Dictionary<string, NandPinNode> Inputs { get; }
    private Dictionary<string, INandTreeNode> Outputs { get; }

    public List<ValidationError> ValidationErrors { get; } = new(); 
    
    public StoredBlueprint(string name, Dictionary<string, NandPinNode> inputs, Dictionary<string, INandTreeNode> outputs)
    {
        Name = name;
        Inputs = inputs;
        Outputs = outputs;

        FuseOutputs(Outputs);
        
        Validate();
    }

    public void Validate()
    {
        var parentNodes = new List<INandTreeNode>();
        
        foreach (var output in Outputs.Values)
        {
            output.Validate(ValidationErrors, Inputs.Values.ToArray(), parentNodes, _copyCounter);
        }

        foreach (var input in Inputs.Values)
        {
            if (input.ValidatedInRun != _copyCounter)
                ValidationErrors.Add(new($"Pin {input.Id} is an unconnected input"));
        }

        _copyCounter++;
    }
    
    private static long _copyCounter;
    public ChipBlueprint Copy()
    {
        if (ValidationErrors.Any()) 
            throw new BlueprintValidationException(Name, ValidationErrors);
        
        var outputs = Outputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Clone(_copyCounter));

        var inputs = Inputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.ClonePin(_copyCounter));

        _copyCounter++;

        return new(Name, inputs, outputs);
    }
    
    private static void FuseOutputs(Dictionary<string, INandTreeNode> outputs)
    {
        foreach (var output in outputs)
        {
            var name = output.Key;
            var fusedOutput = output.Value.Fuse(_copyCounter);

            outputs[name] = fusedOutput;
        }
    }
}

public class BlueprintValidationException : Exception
{
    public BlueprintValidationException(string name, List<ValidationError> errors) :
        base(
            $"Error validating " +
            $"{name} " +
            $"{Environment.NewLine}{string.Join(Environment.NewLine, errors.Select(err => err.Message))}")
    {
        
    }
}