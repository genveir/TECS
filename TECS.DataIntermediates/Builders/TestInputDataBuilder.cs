using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;

namespace TECS.DataIntermediates.Builders;

public class TestInputDataBuilder<TReceiver>
{
    private readonly int _order;
    private readonly Func<TestInputData, TReceiver> _addTest;

    private readonly List<TestSetData> _sets = new();
    
    public static TestInputDataBuilder<TestInputData> CreateBasic(int order) => new(order, tid => tid);

    public static TestInputDataBuilder<TReceiver> 
        WithReceiver(int order, Func<TestInputData, TReceiver> receiver) => new(order, receiver);
    
    private TestInputDataBuilder(int order, Func<TestInputData, TReceiver> addTest)
    {
        _order = order;
        _addTest = addTest;
    }

    public TestInputDataBuilder<TReceiver> AddInput(string inputToSet, bool[] value)
    {
        var group = new NamedNodeGroupName(inputToSet);
        var bv = new BitValue(value);

        var setData = new TestSetData(group, bv);

        _sets.Add(setData);

        return this;
    }

    public TestInputDataBuilder<TReceiver> AddInput(string inputToSet, string value)
    {
        if (!value.All(c => c is '0' or '1'))
            throw new ArgumentException($"string value row {value} contains characters that are not 0 or 1");

        var asBools = value.Select(c => c == '1').Reverse().ToArray();

        return AddInput(inputToSet, asBools);
    }

    public TReceiver Build()
    {
        var testData = new TestInputData(_order, _sets.ToArray());

        return _addTest(testData);
    }
}