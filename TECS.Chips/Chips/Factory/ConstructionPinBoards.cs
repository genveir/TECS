using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

namespace TECS.HDLSimulator.Chips.Factory;

internal class ConstructionPinBoards
{
    private readonly Dictionary<NamedNodeGroupName, PinBoard> _inputs;
    private readonly Dictionary<NamedNodeGroupName, PinBoard> _outputs;
    private readonly Dictionary<NamedNodeGroupName, PinBoard> _internals;

    public ConstructionPinBoards(ChipData chipData)
    {
        _inputs = MapGroups(chipData.In);
        _outputs = MapGroups(chipData.Out);
        _internals = new();
    }

    private Dictionary<NamedNodeGroupName, PinBoard> MapGroups(NamedNodeGroupData[] data) => 
        data.ToDictionary(d => d.Name, d => new PinBoard(d.Name, d.Size));

    public void Link(ChipBlueprint partBlueprint, LinkData link)
    {
        var internalPinDescriptor = link.Internal;
        var externalPinDescriptor = link.External;
    }
}