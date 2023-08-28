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
        
        var inputs = MapInputGroup(chipData.In);
        var outputs = MapOutputGroup(chipData.Out);

        blueprint = new StoredBlueprint(name, inputs, outputs);

        _blueprints[name] = blueprint;
        return blueprint;
    }

    private Dictionary<NamedNodeGroupName, NamedInputNodeGroup> MapInputGroup(NamedNodeGroupData[] data) => 
        data.ToDictionary(d => d.Name, d => new NamedInputNodeGroup(d.Name, d.Size));
    
    private Dictionary<NamedNodeGroupName, NamedOutputNodeGroup> MapOutputGroup(NamedNodeGroupData[] data) => 
        data.ToDictionary(d => d.Name, d => new NamedOutputNodeGroup(d.Name, d.Size));
}