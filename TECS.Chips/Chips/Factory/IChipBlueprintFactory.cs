using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;

namespace TECS.HDLSimulator.Chips.Factory;

public interface IChipBlueprintFactory
{
    FactoryResult<ChipBlueprint> GetBlueprint(ChipName name);
}