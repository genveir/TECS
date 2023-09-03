using System;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Test;

public class CompareData
{
    public ColumnData[] ColumnsToCheck { get; }
    
    public string[][] Values { get; }
    
    internal CompareData(ColumnData[] columnsToCheck, string[][] values)
    {
        if (columnsToCheck.Length == 0)
            throw new ArgumentException("compare data has no groups to check");

        if (values.Length == 0)
            throw new ArgumentException("compare data has no values to check");
        
        CheckGroupForDoubles(columnsToCheck, "compare targets");
        
        foreach (var valueArray in values)
        {
            if (valueArray.Length != columnsToCheck.Length)
                throw new ArgumentException("compare values do not have a value for every target");
        }
        
        ColumnsToCheck = columnsToCheck;
        Values = values;
        
    }
    
    private void CheckGroupForDoubles(ColumnData[] columns, string groupName)
    {
        var distinctCount = columns
            .Select(c => c.Name)
            .Select(g => g.Value)
            .Distinct().Count();

        if (distinctCount != columns.Length)
            throw new ArgumentException($"{groupName} contain double entry");
    }
}