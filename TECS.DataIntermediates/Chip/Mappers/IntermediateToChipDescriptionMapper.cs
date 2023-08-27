using TECS.HDLSimulator.ChipDescriptions;

namespace TECS.DataIntermediates.Chip.Mappers;

public static class IntermediateToChipDescriptionMapper
{
    public static ChipDescription Map(ChipData chipData)
    {
        return new(
            chipData.Name.Value, 
            new(), 
            new(), 
            new());
    }
}