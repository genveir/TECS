using TECS.DataIntermediates.Chip;
using TECS.HDLSimulator.Chips.Chips;

namespace TECS.HDLSimulator.Chips.Factory;

public interface IChipBlueprintFactory
{
    StoredBlueprint CreateBlueprint(ChipData chipData);
}