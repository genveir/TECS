using System.Collections.Generic;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests;

public class VeryTemporaryFactory : IChipBluePrintFactory
{
    public StoredBlueprint CreateBlueprint(ChipData chipData)
    {
        return new(
            name: chipData.Name,
            inputs: new Dictionary<NamedNodeGroupName, NamedNodeGroup>(),
            outputs: new Dictionary<NamedNodeGroupName, NamedNodeGroup>());
    }
}