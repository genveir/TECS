using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Chip;

public class ChipPartData
{
    public ChipName PartName { get; }
    
    public LinkData[] Links { get; }

    public ChipPartData(ChipName partName, IEnumerable<LinkData> links)
    {
        var linkArray = links.ToArray();

        if (linkArray.Length == 0)
            throw new ArgumentException($"part {partName} has no links");
        
        PartName = partName;
        Links = linkArray;
    }
}