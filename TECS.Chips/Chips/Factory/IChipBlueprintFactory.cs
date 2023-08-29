using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

public interface IChipBlueprintFactory
{
    FactoryResult<ChipBlueprint> GetBlueprint(ChipName name);
}