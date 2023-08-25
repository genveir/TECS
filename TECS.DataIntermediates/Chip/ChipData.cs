using System;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Chip;

public class ChipData
{
    public ChipName Name { get; }
    
    public NamedNodeGroupName[] In { get; }
    
    public NamedNodeGroupName[] Out { get; }
    
    public ChipPartData[] Parts { get; }

    internal ChipData(ChipName name, NamedNodeGroupName[] inGroups, NamedNodeGroupName[] outGroups, 
        ChipPartData[] parts)
    {
        Name = name;

        CheckGroupForDoubles(inGroups, "inputs");
        CheckGroupForDoubles(outGroups, "outputs");
        CheckGroupForDoubles(inGroups.Concat(outGroups).ToArray(), "inputs and outputs");

        if (inGroups.Length == 0)
            throw new ArgumentException("chip has no inputs");

        if (outGroups.Length == 0)
            throw new ArgumentException("chip has no outputs");
        
        In = inGroups;
        Out = outGroups;
        Parts = parts;
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