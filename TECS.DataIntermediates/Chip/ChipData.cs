using System.Collections.Generic;
using TECS.DataIntermediates.Chip.Names;

namespace TECS.DataIntermediates.Chip;

public class ChipData
{
    public ChipName Name { get; }
    
    public IEnumerable<NamedNodeGroupName> In { get; }
    
    public IEnumerable<NamedNodeGroupName> Out { get; }
    
    public IEnumerable<ChipPartData> Parts { get; }

    public ChipData(ChipName name, IEnumerable<NamedNodeGroupName> inGroups, IEnumerable<NamedNodeGroupName> outGroups, 
        IEnumerable<ChipPartData> parts)
    {
        Name = name;
        In = inGroups;
        Out = outGroups;
        Parts = parts;
    }
}