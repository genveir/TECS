using System;
using System.Linq;
using TECS.DataIntermediates.Chip;

namespace TECS.DataIntermediates.Test;

public class TestData
{
    public ChipData ChipToTest { get; }
    
    public CompareData ExpectedValues { get; }
    
    public TestInputData[] Tests { get; }

    internal TestData(ChipData chipToTest, CompareData expectedValues, TestInputData[] tests)
    {
        if (tests.Length == 0)
            throw new ArgumentException("test set is empty");

        if (expectedValues.Values.Length != tests.Length)
            throw new ArgumentException("expected values count differs from number of tests");

        if (tests.Length != tests.Select(t => t.Order).Distinct().Count())
            throw new ArgumentException("orders on tests must be distinct");

        ChipToTest = chipToTest;
        ExpectedValues = expectedValues;
        Tests = tests;
    }
}