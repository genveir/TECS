using System;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Test;

public class CompareData
{
    public NamedNodeGroupName[] GroupsToCheck { get; }
    
    public BitValue[][] Values { get; }
    
    public BitSize[] ColumnSizes { get; }

    internal CompareData(NamedNodeGroupName[] groupsToCheck, BitValue[][] values)
    {
        if (groupsToCheck.Length == 0)
            throw new ArgumentException("compare data has no groups to check");

        if (values.Length == 0)
            throw new ArgumentException("compare data has no values to check");
        
        CheckGroupForDoubles(groupsToCheck, "compare targets");

        ColumnSizes = values[0].Select(bv => bv.Size).ToArray();
        foreach (var valueArray in values)
        {
            if (valueArray.Length != groupsToCheck.Length)
                throw new ArgumentException("compare values do not have a value for every target");
            
            for (int n = 0; n < valueArray.Length; n++)
                if (!valueArray[n].Size.Equals(ColumnSizes[n]))
                    throw new ArgumentException("compare values are not consistently sized");
        }
        
        GroupsToCheck = groupsToCheck;
        Values = values;
        
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