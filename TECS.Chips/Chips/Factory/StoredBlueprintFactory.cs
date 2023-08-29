using TECS.DataIntermediates.Chip;
using TECS.HDLSimulator.Chips.Chips;

namespace TECS.HDLSimulator.Chips.Factory;

internal class StoredBlueprintFactory
{
    private readonly ChipBlueprintFactory _factory;

    internal StoredBlueprintFactory(ChipBlueprintFactory factory)
    {
        _factory = factory;
    }
    
    internal FactoryResult<StoredBlueprint> CreateBlueprint(ChipData chipData)
    {
        var name = chipData.Name;
        
        var constructionPinBoards = new ConstructionPinBoards(chipData);

        foreach (var partData in chipData.Parts)
        {
            var getPartBlueprintResult = _factory.GetBlueprint(partData.PartName);
            if (!getPartBlueprintResult.Success)
                return FactoryResult<StoredBlueprint>.Fail(getPartBlueprintResult.Errors);

            var partBlueprint = getPartBlueprintResult.Result;
            LinkPart(constructionPinBoards, partData, partBlueprint!);
        }

        var blueprint = new StoredBlueprint(name, new(), new());

        return FactoryResult<StoredBlueprint>.Succeed(blueprint);
    }

    private void LinkPart(ConstructionPinBoards constructionPinBoards, ChipPartData part, ChipBlueprint partBlueprint)
    {
        foreach (var link in part.Links)
        {
            constructionPinBoards.Link(partBlueprint, link);
        }
    }
}