using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Factory;

public class DebugChipFactory
{
    private readonly ChipBlueprintFactory _factory;

    internal DebugChipFactory(ChipBlueprintFactory factory)
    {
        _factory = factory;
    }
    
    internal FactoryResult<DebugChip> Create(ChipData chipData)
    {
        var constructionPinBoards = new ConstructionPinBoards(chipData);

        foreach (var partData in chipData.Parts)
        {
            var getPartBlueprintResult = _factory.GetBlueprint(partData.PartName);
            if (!getPartBlueprintResult.Success)
                return FactoryResult<DebugChip>.Fail(getPartBlueprintResult.Errors);

            var partBlueprint = getPartBlueprintResult.Result;
            LinkPart(constructionPinBoards, partData, partBlueprint!);
        }

        var inputs = constructionPinBoards.Inputs.ToDictionary(
            kvp => kvp.Key,
            kvp => new InputNodeGroup(kvp.Value));
        var internals = constructionPinBoards.Internals.ToDictionary(
            kvp => kvp.Key,
            kvp => new OutputNodeGroup(kvp.Value));
        var outputs = constructionPinBoards.Outputs.ToDictionary(
            kvp => kvp.Key,
            kvp => new OutputNodeGroup(kvp.Value));

        var debugChip = new DebugChip(chipData.Name, inputs, outputs, internals);

        return FactoryResult<DebugChip>.Succeed(debugChip);
    }

    private void LinkPart(ConstructionPinBoards constructionPinBoards, ChipPartData part, ChipBlueprint partBlueprint)
    {
        foreach (var link in part.Links)
        {
            constructionPinBoards.Link(partBlueprint, link);
        }
    }
}