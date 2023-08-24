using System;
using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.ChipDescriptions;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

public class ChipBlueprintFactory
{
    private readonly IEnumerable<ChipDescription> _descriptions;
    private readonly Dictionary<string, StoredBlueprint> _blueprints = new();

    public ChipBlueprintFactory(IEnumerable<ChipDescription> descriptions)
    {
        _descriptions = descriptions;
        
        _blueprints.Add("Nand", NandBlueprint());
    }

    public ChipDescription? GetChipDescription(string name) =>
        _descriptions.SingleOrDefault(desc => desc.Name == name);

    public StoredBlueprint BuildBlueprint(ChipDescription description)
    {
        if (_blueprints.TryGetValue(description.Name, out var result)) 
            return result;

        var constructionPins = ResolvePins(description);

        foreach (var partDescription in description.Parts)
        {
            var partBlueprint = ResolvePart(partDescription);

            LinkPart(partDescription, partBlueprint, constructionPins);
        }

        var bluePrint = new StoredBlueprint(description.Name, constructionPins.InputPins, constructionPins.OutputPins);
        _blueprints.Add(description.Name, bluePrint);

        return bluePrint;
    }

    private ChipBlueprint ResolvePart(PartDescription description)
    {
        if (!_blueprints.TryGetValue(description.Name, out var result))
        {
            var chipDescription = _descriptions.Single(desc => desc.Name == description.Name);

            BuildBlueprint(chipDescription);

            result = _blueprints[description.Name];
        }

        return result.CopyToBlueprintInstance();
    }

    private void LinkPart(PartDescription description, ChipBlueprint partBlueprint, ConstructionPins pins)
    {
        foreach (var pinConnection in description.PinConnections)
        {
            var internalPinsAreInput = partBlueprint.Inputs.ContainsKey(pinConnection.Local.Name);
            var internalPins = internalPinsAreInput
                ? PinLinkGroupHelper.GetPins(partBlueprint.Inputs[pinConnection.Local.Name], pinConnection.Local)
                : PinLinkGroupHelper.GetPins(partBlueprint.Outputs[pinConnection.Local.Name], pinConnection.Local);

            var externalPinGroup = pins.GetNodeGroup(pinConnection.External.Name) ??
                pins.CreateInternalNodeGroup(pinConnection.External.Name, internalPins.Count);
            var externalPins = PinLinkGroupHelper.GetPins(externalPinGroup, pinConnection.External);

            if (internalPins.Count != externalPins.Count)
                throw new InvalidOperationException($"invalid connection {pinConnection}");
            
            if (internalPinsAreInput)
            {
                for (int n = 0; n < internalPins.Count; n++)
                    switch (internalPins[n])
                    {
                        case NandPinNode pin:
                            pin.Parent = externalPins[n];
                            break;
                        default:
                            throw new InvalidOperationException($"Cannot link non-pin {internalPins[n]} as input");
                    }
            }
            else
            {
                for (int n = 0; n < internalPins.Count; n++)
                    switch (externalPins[n])
                    {
                        case NandPinNode pin:
                            pin.Parent = internalPins[n];
                            break;
                        default:
                            throw new InvalidOperationException($"Cannot link non-pin {internalPins[n]} as input");
                    }
            }
        }
    }
    
    private static ConstructionPins ResolvePins(ChipDescription description)
    {
        var inputGroups = new Dictionary<string, NamedNodeGroup>();
        foreach (var namedPinGroupDescription in description.In)
        {
            inputGroups.Add(namedPinGroupDescription.Name,
                new(namedPinGroupDescription.Name, namedPinGroupDescription.BitSize));
        }

        var outputGroups = new Dictionary<string, NamedNodeGroup>();
        foreach (var namedPinGroupDescription in description.Out)
        {
            outputGroups.Add(namedPinGroupDescription.Name,
                new(namedPinGroupDescription.Name, namedPinGroupDescription.BitSize));
        }

        var internalGroups = new Dictionary<string, NamedNodeGroup>();

        return new(inputGroups, outputGroups, internalGroups);
    }

    private static StoredBlueprint NandBlueprint()
    {
        var nandNode = new NandNode();

        nandNode.TryGetInputPins(out var inputNodes);

        var aGroup = new NamedNodeGroup("a", 1)
        {
            Nodes = { [0] = inputNodes.a }
        };

        var bGroup = new NamedNodeGroup("b", 1)
        {
            Nodes = { [0] = inputNodes.b }
        };

        var inputs = new Dictionary<string, NamedNodeGroup>
        {
            { "a", aGroup },
            { "b", bGroup }
        };

        var outGroup = new NamedNodeGroup("out", 1)
        {
            Nodes = { [0] = nandNode }
        };

        var outputs = new Dictionary<string, NamedNodeGroup>()
        {
            { "out", outGroup }
        };

        return new StoredBlueprint("Nand", inputs, outputs);
    }
}