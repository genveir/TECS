using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Test;

public class TestData
{
    public ChipName ChipToTest { get; }
    
    public CompareData ExpectedValues { get; }
    
    public OutputListData[] OutputList { get; }
    
    public TestInputData[] Tests { get; }

    public TestData(ChipName chipToTest, CompareData expectedValues, OutputListData[] outputList, TestInputData[] tests)
    {
        for (int n = 0; n < outputList.Length; n++)
        {
            if (!expectedValues.GroupsToCheck[n].Equals(outputList[n].Group))
                throw new ArgumentException(
                    $"output list {outputList[n]} and compare target {expectedValues.GroupsToCheck[n]} do not match by name");

            if (!outputList[n].BitSize.Equals(expectedValues.ColumnSizes[n]))
                throw new ArgumentException(
                    $"output list and compare targets do not match on bit size for {outputList[n].Group}");
        }

        ChipToTest = chipToTest;
        ExpectedValues = expectedValues;
        OutputList = outputList;
        Tests = tests;
    }
}