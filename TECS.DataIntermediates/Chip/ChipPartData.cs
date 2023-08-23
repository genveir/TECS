using System.Collections.Generic;
using TECS.DataIntermediates.Chip.Names;

namespace TECS.DataIntermediates.Chip;

public class ChipPartData
{
    public ChipName PartName { get; }
    
    public IEnumerable<LinkData> Links { get; }

    public ChipPartData(ChipName partName, IEnumerable<LinkData> links)
    {
        PartName = partName;
        Links = links;
    }
}