using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;

namespace TECS.HDLSimulator.Chips.Factory;

public class ChipBlueprintFactory : IChipBlueprintFactory
{
    private readonly IEnumerable<ChipData> _allChipData;
    private readonly Dictionary<ChipName, StoredBlueprint> _blueprints = new();

    public ChipBlueprintFactory(IEnumerable<ChipData> allChipData)
    {
        _allChipData = allChipData;
    }

    public StoredBlueprint CreateBlueprint(ChipData chipData)
    {
        var name = chipData.Name;
        
        if (_blueprints.TryGetValue(chipData.Name, out var blueprint))
            return blueprint;
        
        var inputs = MapGroup(chipData.In);
        var outputs = MapGroup(chipData.Out);

        blueprint = new StoredBlueprint(name, inputs, outputs);

        _blueprints[name] = blueprint;
        return blueprint;
    }

    private Dictionary<NamedNodeGroupName, NamedNodeGroup> MapGroup(NamedNodeGroupData[] data) => 
        data.ToDictionary(d => d.Name, d => new NamedNodeGroup(d.Name, d.Size));
}