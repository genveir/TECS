using System.Collections.Generic;
using System.IO;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprintFactory
{
    private readonly IEnumerable<ChipDescription> _descriptions;
    private readonly Dictionary<string, ChipBlueprint> _blueprints = new();

    public static ChipBlueprintFactory FromFilesystem(string dataFolder)
    {
        var folder = new HdlFolder(Path.Combine(dataFolder, "HDL"));

        var files = folder.GetFiles();

        var contents = files.Select(f => f.GetContents());

        var descriptions = contents.Select(HdlParser.ParseDescription).ToArray();

        return new ChipBlueprintFactory(descriptions);
    }
    
    private ChipBlueprintFactory(IEnumerable<ChipDescription> descriptions)
    {
        _descriptions = descriptions;
        
        _blueprints.Add("Nand", NandBlueprint());
    }

    public ChipDescription? GetChipDescription(string name) =>
        _descriptions.SingleOrDefault(desc => desc.Name == name);

    public ChipBlueprint BuildBlueprint(ChipDescription description)
    {
        if (_blueprints.TryGetValue(description.Name, out var result)) 
            return result;

        var constructionPins = ResolvePins(description);

        foreach (var partDescription in description.Parts)
        {
            var partBlueprint = ResolvePart(partDescription);

            LinkPart(partDescription, partBlueprint, constructionPins);
        }
        
        var outputs = constructionPins.OutputPins.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value as INandTreeNode);
        
        var bluePrint = new ChipBlueprint(description.Name, constructionPins.InputPins, outputs);
        _blueprints.Add(description.Name, bluePrint);

        return bluePrint;
    }

    private ChipBlueprint ResolvePart(PartDescription description)
    {
        if (_blueprints.TryGetValue(description.Name, out var blueprint)) return blueprint.Clone();

        var chipDescription = _descriptions.Single(desc => desc.Name == description.Name);

        return BuildBlueprint(chipDescription);
    }

    private void LinkPart(PartDescription description, ChipBlueprint partBlueprint, ConstructionPins pins)
    {
        foreach (var pinConnection in description.PinConnections)
        {
            var externalPin = pins.GetPin(pinConnection.External) ?? 
                              pins.CreateInternalPin(pinConnection.External);
            
            if (partBlueprint.Inputs.TryGetValue(pinConnection.Local, out NandPinNode? pin))
            {
                pin.Parent = externalPin;
            }
            else
            {
                externalPin.Parent = partBlueprint.Outputs[pinConnection.Local];
            }
        }
    }

    private class ConstructionPins
    {
        public Dictionary<string, NandPinNode> InputPins { get; }
        public Dictionary<string, NandPinNode> OutputPins { get; }
        public Dictionary<string, NandPinNode> InternalPins { get; }

        public NandPinNode? GetPin(string name)
        {
            if (InputPins.TryGetValue(name, out NandPinNode? pin)) return pin;
            if (OutputPins.TryGetValue(name, out pin)) return pin;
            if (InternalPins.TryGetValue(name, out pin)) return pin;

            return pin;
        }

        public NandPinNode CreateInternalPin(string name)
        {
            var newPin = new NandPinNode();
            InternalPins.Add(name, newPin);

            return newPin;
        }
        
        public ConstructionPins(Dictionary<string, NandPinNode> inputPins, Dictionary<string, NandPinNode> outputPins, Dictionary<string, NandPinNode> internalPins)
        {
            InputPins = inputPins;
            OutputPins = outputPins;
            InternalPins = internalPins;
        }
    }

    private static ConstructionPins ResolvePins(ChipDescription description)
    {
        var inputPins = new Dictionary<string, NandPinNode>();
        foreach (var pinDescription in description.In)
        {
            inputPins.Add(pinDescription.Name, new NandPinNode());
        }

        var outputPins = new Dictionary<string, NandPinNode>();
        foreach (var pinDescription in description.Out)
        {
            outputPins.Add(pinDescription.Name, new NandPinNode());
        }

        var internalPins = new Dictionary<string, NandPinNode>();

        return new(inputPins, outputPins, internalPins);
    }

    internal static ChipBlueprint NandBlueprint()
    {
        var nandNode = new NandNode();

        nandNode.TryGetInputPins(out var inputNodes);
        
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", inputNodes.a },
            { "b", inputNodes.b }
        };

        var outputs = new Dictionary<string, INandTreeNode>()
        {
            { "out", nandNode }
        };

        return new ChipBlueprint("Nand", inputs, outputs);
    }
}