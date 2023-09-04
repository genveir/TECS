using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Test;

namespace TECS.DataIntermediates.Builders;

public class TestDataBuilder
{
    private ChipData? _chipToTest;
    private CompareData? _expectedValue;
    private readonly List<TestInputData> _tests = new();

    public TestDataBuilder WithChipToTest(ChipData chipData)
    {
        _chipToTest = chipData;

        return this;
    }
    
    public CompareDataBuilder<TestDataBuilder> WithExpectedValues()
    {
        return CompareDataBuilder<TestDataBuilder>.WithReceiver(WithExpectedValues);
    }

    public TestDataBuilder WithExpectedValues(CompareData compareData)
    {
        _expectedValue = compareData;

        return this;
    }

    public TestInputDataBuilder<TestDataBuilder> AddTest(int order)
    {
        return TestInputDataBuilder<TestDataBuilder>.WithReceiver(order, AddTest);
    }

    public TestDataBuilder AddTest(TestInputData test)
    {
        _tests.Add(test);

        return this;
    }

    public TestDataBuilder WithTests(IEnumerable<TestInputData> tests)
    {
        _tests.Clear();
        _tests.AddRange(tests);

        return this;
    }

    public TestData Build()
    {
        if (_chipToTest == null)
            throw new BuilderException("cannot build test data without a chip to test");
        if (_expectedValue == null)
            throw new BuilderException("cannot build test data without compare data");
        
        var tests = _tests.ToArray();
        
        return new(_chipToTest, _expectedValue, tests);
    }
}

public class BuilderException : Exception
{
    public BuilderException(string message) : base(message)
    {
        
    }
}