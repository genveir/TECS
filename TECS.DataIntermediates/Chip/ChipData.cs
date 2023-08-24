using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip.Names;

namespace TECS.DataIntermediates.Chip;

public class ChipData
{
    public ChipName Name { get; }
    
    public NamedNodeGroupName[] In { get; }
    
    public NamedNodeGroupName[] Out { get; }
    
    public ChipPartData[] Parts { get; }

    public ChipData(ChipName name, IEnumerable<NamedNodeGroupName> inGroups, IEnumerable<NamedNodeGroupName> outGroups, 
        IEnumerable<ChipPartData> parts)
    {
        Name = name;

        var inGroupArray = inGroups.ToArray();
        var outGroupArray = outGroups.ToArray();
        var partsArray = parts.ToArray();

        CheckGroupForDoubles(inGroupArray, "inputs");
        CheckGroupForDoubles(outGroupArray, "outputs");
        CheckGroupForDoubles(inGroupArray.Concat(outGroupArray).ToArray(), "inputs and outputs");

        if (inGroupArray.Length == 0)
            throw new ArgumentException("chip has no inputs");

        if (outGroupArray.Length == 0)
            throw new ArgumentException("chip has no outputs");
        
        In = inGroupArray;
        Out = outGroupArray;
        Parts = partsArray;
    }

    private void CheckGroupForDoubles(NamedNodeGroupName[] group, string groupName)
    {
        var distinctCount = group
            .Select(g => g.Value)
            .Distinct().Count();

        if (distinctCount != group.Length)
            throw new ArgumentException($"{groupName} contain double entry");
    }
}