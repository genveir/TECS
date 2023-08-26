using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;

namespace TECS.DataIntermediates.Builders;

public class TestDataBuilder
{
    private string? _chipToTest;
    private CompareData? _expectedValue;
    private readonly List<OutputListData> _outputs;
    private readonly List<TestInputData> _tests;

    public TestDataBuilder()
    {
        _outputs = new();
        _tests = new();
    }

    public TestDataBuilder WithChipToTest(string chipName)
    {
        _chipToTest = chipName;

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

    public CompareDataBuilder<TestDataBuilder> AddCompareData()
    {
        return CompareDataBuilder<TestDataBuilder>.WithReceiver(AddCompareData);
    }

    private TestDataBuilder AddCompareData(CompareData compareData)
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
        var name = new ChipName(_chipToTest ?? "");
        if (_expectedValue == null)
            throw new BuilderException("cannot build test data without compare data");

        var outputs = _outputs.ToArray();
        var tests = _tests.ToArray();
        
        return new(name, _expectedValue, outputs, tests);
    }
}

public class BuilderException : Exception
{
    public BuilderException(string message) : base(message)
    {
        
    }
}