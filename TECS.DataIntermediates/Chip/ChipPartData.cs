using System;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Chip;

public class ChipPartData
{
    public ChipName PartName { get; }
    
    public LinkData[] Links { get; }

    internal ChipPartData(ChipName partName, LinkData[] links)
    {
        var linkArray = links.ToArray();

        if (linkArray.Length == 0)
            throw new ArgumentException($"part {partName} has no links");
        
        PartName = partName;
        Links = linkArray;
    }
}