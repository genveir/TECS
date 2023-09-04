using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Builders;

public class SimpleTestInputDataBuilder : TestInputDataBuilder<TestInputData>
{
    public SimpleTestInputDataBuilder(int order) : base(order, tid => tid) { }
}

public class TestInputDataBuilder<TReceiver>
{
    private readonly int _order;
    private readonly Func<TestInputData, TReceiver> _addTest;

    private readonly List<TestSetData> _sets = new();

    public static TestInputDataBuilder<TReceiver> 
        WithReceiver(int order, Func<TestInputData, TReceiver> receiver) => new(order, receiver);
    
    protected TestInputDataBuilder(int order, Func<TestInputData, TReceiver> addTest)
    {
        _order = order;
        _addTest = addTest;
    }

    public TestInputDataBuilder<TReceiver> AddInput(string inputToSet, bool value) =>
        AddInput(inputToSet, new BitValue(value));

    public TestInputDataBuilder<TReceiver> AddInput(string inputToSet, string value, int size) => 
        AddInput(inputToSet, new BitValue(value, size));

    public TestInputDataBuilder<TReceiver> AddInput(string inputToSet, short value, int size) =>
        AddInput(inputToSet, new ShortValue(value, size).AsBitValue());

    private TestInputDataBuilder<TReceiver> AddInput(string inputToSet, BitValue bitValue)
    {
        var group = new NamedNodeGroupName(inputToSet);
        
        var setData = new TestSetData(group, bitValue);

        _sets.Add(setData);

        return this;
    }

    public TReceiver Build()
    {
        var testData = new TestInputData(_order, _sets.ToArray());

        return _addTest(testData);
    }
}