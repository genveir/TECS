using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Builders;

public class TestDataBuilder
{
    private ChipData? _chipToTest;
    private CompareData? _expectedValue;
    private readonly List<OutputListData> _outputs;
    private readonly List<TestInputData> _tests;

    public TestDataBuilder()
    {
        _outputs = new();
        _tests = new();
    }

    public TestDataBuilder WithChipToTest(ChipData chipData)
    {
        _chipToTest = chipData;

        return this;
    }

    public TestDataBuilder AddOutput(string name, int bitSize)
    {
        var outputName = new NamedNodeGroupName(name);
        var bs = new BitSize(bitSize);
        
        var outputListItem = new OutputListData(outputName, bs);
        
        _outputs.Add(outputListItem);

        return this;
    }

    public CompareDataBuilder<TestDataBuilder> SetExpectedValues()
    {
        return CompareDataBuilder<TestDataBuilder>.WithReceiver(SetExpectedValues);
    }

    private TestDataBuilder SetExpectedValues(CompareData compareData)
    {
        _expectedValue = compareData;

        return this;
    }

    public TestInputDataBuilder<TestDataBuilder> AddTest(int order)
    {
        return TestInputDataBuilder<TestDataBuilder>.WithReceiver(order, AddTest);
    }

    private TestDataBuilder AddTest(TestInputData test)
    {
        _tests.Add(test);

        return this;
    }

    public TestData Build()
    {
        if (_chipToTest == null)
            throw new BuilderException("cannot build test data without a chip to test");
        if (_expectedValue == null)
            throw new BuilderException("cannot build test data without compare data");

        var outputs = _outputs.ToArray();
        var tests = _tests.ToArray();
        
        return new(_chipToTest, _expectedValue, outputs, tests);
    }
}

public class BuilderException : Exception
{
    public BuilderException(string message) : base(message)
    {
        
    }
}