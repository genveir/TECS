using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

internal class ConstructionPinBoards
{
    public Dictionary<NamedNodeGroupName, PinBoard> Inputs { get; }
    public Dictionary<NamedNodeGroupName, PinBoard> Outputs { get; }
    public Dictionary<NamedNodeGroupName, PinBoard> Internals { get; }

    public ConstructionPinBoards(ChipData chipData)
    {
        Inputs = MapGroups(chipData.In);
        Outputs = MapGroups(chipData.Out);
        Internals = new();
    }

    private Dictionary<NamedNodeGroupName, PinBoard> MapGroups(NamedNodeGroupData[] data) => 
        data.ToDictionary(d => d.Name, d => new PinBoard(d.Name, d.Size));

    public void Link(ChipBlueprint partBlueprint, LinkData link)
    {
        var internalName = link.Internal.Name as NamedNodeGroupName;
        if (partBlueprint.Inputs.ContainsKey(internalName))
            LinkPartInput(partBlueprint, link);
        else if (partBlueprint.Outputs.ContainsKey(internalName))
            LinkPartOutput(partBlueprint, link);
        else
            throw new InvalidOperationException($"Part does not have named node group {internalName}");
    }

    private void LinkPartInput(ChipBlueprint partBlueprint, LinkData link)
    {
        var inputGroup = partBlueprint.Inputs[link.Internal.Name];
        var inputLowerBound = link.Internal.LowerPin ?? 0;
        var inputUpperBound = link.Internal.UpperPin ?? inputGroup.Nodes.Length - 1;

        var externalName = link.External.Name as NamedNodeGroupName;

        switch (externalName.Value)
        {
            case "true": 
                LinkConstant(inputGroup, inputLowerBound, inputUpperBound, true);
                break;
            case "false":
                LinkConstant(inputGroup, inputLowerBound, inputUpperBound, false);
                break;
            case "clk":
                LinkClock(inputGroup, inputLowerBound, inputUpperBound);
                break;
            default:
                LinkPinBoardInput(inputGroup, inputLowerBound, inputUpperBound, link.External);
                break;
        }
        
        if (externalName.Value == "true" || externalName.Value == "false")
            LinkConstant(inputGroup, inputLowerBound, inputUpperBound, externalName.Value == "true");
        else
            LinkPinBoardInput(inputGroup, inputLowerBound, inputUpperBound, link.External);
    }

    private void LinkConstant(InputNodeGroup inputGroup, int inputLowerBound, int inputUpperBound, bool constantValue)
    {
        for (int n = inputLowerBound; n <= inputUpperBound; n++)
        {
            inputGroup.Pins[n].Parent = constantValue ? ConstantPin.True : ConstantPin.False;
        }
    }

    private void LinkClock(InputNodeGroup inputGroup, int inputLowerBound, int inputUpperBound)
    {
        for (int n = inputLowerBound; n <= inputUpperBound; n++)
        {
            inputGroup.Pins[n].Parent = ClockPin.Instance;
        }
    }

    private void LinkPinBoardInput(InputNodeGroup inputGroup, int inputLowerBound, int inputUpperBound, ExternalLinkData externalLink)
    {
        var externalName = externalLink.Name;
        
        if (!Inputs.TryGetValue(externalName, out PinBoard? externalGroup) && 
            !Internals.TryGetValue(externalName, out externalGroup))
        {
            if (Outputs.ContainsKey(externalName))
                throw new InvalidOperationException(
                    $"Cannot link part input {inputGroup.Name} to external output {externalName}");

            externalGroup = new PinBoard(externalName, size: new(inputUpperBound - inputLowerBound + 1));
            
            Internals.Add(externalName, externalGroup);
        }
        
        var externalLowerBound = externalLink.LowerPin ?? 0;
        var offset = externalLowerBound - inputLowerBound;
        
        for (int n = inputLowerBound; n <= inputUpperBound; n++)
        {
            inputGroup.Pins[n].Parent = externalGroup.Nodes[n + offset];
        }
    }

    private void LinkPartOutput(ChipBlueprint partBlueprint, LinkData link)
    {
        var outputGroup = partBlueprint.Outputs[link.Internal.Name];
        var outputLowerBound = link.Internal.LowerPin ?? 0;
        var outputUpperBound = link.Internal.UpperPin ?? outputGroup.Nodes.Length - 1;

        var externalName = link.External.Name as NamedNodeGroupName;

        if (!Outputs.TryGetValue(externalName, out PinBoard? externalGroup) &&
            !Internals.TryGetValue(externalName, out externalGroup))
        {
            if (Inputs.ContainsKey(externalName))
                throw new InvalidOperationException(
                    $"Cannot link part output {link.Internal.Name} to external input {link.External.Name}");

            externalGroup = new PinBoard(externalName, size: new(outputUpperBound - outputLowerBound + 1));
            
            Internals.Add(link.External.Name, externalGroup);
        }

        var externalLowerBound = link.External.LowerPin ?? 0;
        var offset = externalLowerBound - outputLowerBound;

        for (int n = outputLowerBound; n <= outputUpperBound; n++)
        {
            externalGroup.Nodes[n + offset].Parent = outputGroup.Nodes[n];
        }
    }
}