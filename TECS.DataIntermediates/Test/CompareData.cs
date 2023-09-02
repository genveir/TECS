using System;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Test;

public class CompareData
{
    public NamedNodeGroupName[] GroupsToCheck { get; }
    
    public IStringFormattableValue[][] Values { get; }
    
    internal CompareData(NamedNodeGroupName[] groupsToCheck, IStringFormattableValue[][] values)
    {
        if (groupsToCheck.Length == 0)
            throw new ArgumentException("compare data has no groups to check");

        if (values.Length == 0)
            throw new ArgumentException("compare data has no values to check");
        
        CheckGroupForDoubles(groupsToCheck, "compare targets");
        
        foreach (var valueArray in values)
        {
            if (valueArray.Length != groupsToCheck.Length)
                throw new ArgumentException("compare values do not have a value for every target");
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