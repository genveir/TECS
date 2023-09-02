using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Test;

public class TestData
{
    public ChipData ChipToTest { get; }
    
    public CompareData ExpectedValues { get; }
    
    public OutputListData[] OutputList { get; }
    
    public TestInputData[] Tests { get; }

    internal TestData(ChipData chipToTest, CompareData expectedValues, OutputListData[] outputList, TestInputData[] tests)
    {
        if (outputList.Length == 0)
            throw new ArgumentException("output list is empty");

        if (tests.Length == 0)
            throw new ArgumentException("test set is empty");

        if (outputList.Length != outputList.Select(ol => ol.Group).Distinct().Count())
            throw new ArgumentException("output list contains duplicate");
        
        Dictionary<NamedNodeGroupName, BitSize> bitSizes = 
            outputList.ToDictionary(ol => ol.Group, ol => ol.BitSize);
        
        for (int n = 0; n < outputList.Length; n++)
        {
            if (!expectedValues.GroupsToCheck[n].Equals(outputList[n].Group))
                throw new ArgumentException(
                    $"output list {outputList[n]} and compare target {expectedValues.GroupsToCheck[n]} do not match by name");
        }

        if (expectedValues.Values.Length != tests.Length)
            throw new ArgumentException("expected values count differs from number of tests");

        if (tests.Length != tests.Select(t => t.Order).Distinct().Count())
            throw new ArgumentException("orders on tests must be distinct");
        
        var setters = tests.SelectMany(t => t.SetData).ToArray();
        for (int n = 0; n < setters.Length; n++)
        {
            if (bitSizes.TryGetValue(setters[n].Group, out var size))
            {
                if (!size.Equals(setters[n].ValueToSet.Size))
                    throw new ArgumentException(
                        "cannot set an input to a size that is specified to be different in output list or expected values");
            }            
        }

        ChipToTest = chipToTest;
        ExpectedValues = expectedValues;
        OutputList = outputList;
        Tests = tests;
    }
}