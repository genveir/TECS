using System;
using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips;

public class StoredBlueprint
{
    private string Name { get; }
    private Dictionary<string, NamedNodeGroup> Inputs { get; }
    private Dictionary<string, NamedNodeGroup> Outputs { get; }

    public List<ValidationError> ValidationErrors { get; } = new(); 
    
    public StoredBlueprint(string name, Dictionary<string, NamedNodeGroup> inputs, Dictionary<string, NamedNodeGroup> outputs)
    {
        Name = name;
        Inputs = inputs;
        Outputs = outputs;

        FuseOutputs(Outputs);
        
        Validate();
    }

    private void Validate()
    {
        foreach (var input in Inputs.Values)
        {
            input.SetAsInputForValidation(ValidationErrors, _copyCounter);
        }
        
        foreach (var output in Outputs.Values)
        {
            output.Validate(ValidationErrors, _copyCounter);
        }

        foreach (var input in Inputs.Values)
        {
            input.IsValidatedInRun(ValidationErrors, _copyCounter);
        }

        _copyCounter++;
    }
    
    private static long _copyCounter;
    public ChipBlueprint CopyToBlueprintInstance()
    {
        if (ValidationErrors.Any()) 
            throw new BlueprintValidationException(Name, ValidationErrors);
        
        var outputs = Outputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Clone(_copyCounter));

        var inputs = Inputs.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Clone(_copyCounter));

        _copyCounter++;

        return new(Name, inputs, outputs);
    }
    
    private static void FuseOutputs(Dictionary<string, NamedNodeGroup> outputs)
    {
        foreach (var output in outputs)
        {
            output.Value.Fuse(_copyCounter);
        }

        _copyCounter++;
    }

    public override string ToString()
    {
        return "StoredBlueprint {Name}";
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