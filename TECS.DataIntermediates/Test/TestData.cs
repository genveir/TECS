using System;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Test;

public class TestData
{
    public ChipName ChipToTest { get; }
    
    public CompareData ExpectedValues { get; }
    
    public OutputListData[] OutputList { get; }
    
    public TestInputData[] Tests { get; }

    internal TestData(ChipName chipToTest, CompareData expectedValues, OutputListData[] outputList, TestInputData[] tests)
    {
        if (outputList.Length == 0)
            throw new ArgumentException("output list is empty");

        if (tests.Length == 0)
            throw new ArgumentException("test set is empty");

        if (outputList.Length != outputList.Select(ol => ol.Group).Distinct().Count())
            throw new ArgumentException("output list contains duplicate");
        
        for (int n = 0; n < outputList.Length; n++)
        {
            if (!expectedValues.GroupsToCheck[n].Equals(outputList[n].Group))
                throw new ArgumentException(
                    $"output list {outputList[n]} and compare target {expectedValues.GroupsToCheck[n]} do not match by name");

            if (!outputList[n].BitSize.Equals(expectedValues.ColumnSizes[n]))
                throw new ArgumentException(
                    $"output list and compare targets do not match on bit size for {outputList[n].Group}");
        }

        if (expectedValues.Values.Length != tests.Length)
            throw new ArgumentException("expected values count differs from number of tests");

        ChipToTest = chipToTest;
        ExpectedValues = expectedValues;
        OutputList = outputList;
        Tests = tests;
    }
}