using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;

namespace TECS.DataIntermediates.Builders;

public class CompareDataBuilder<TReceiver>
{
    private readonly Func<CompareData, TReceiver> _addExpected;
    private readonly List<bool[][]> _values = new();
    
    private string[] _groupNames = Array.Empty<string>();

    public static CompareDataBuilder<CompareData> CreateBasic() => new(cpd => cpd);

    public static CompareDataBuilder<TReceiver> 
        WithReceiver(Func<CompareData, TReceiver> receiver) => new(receiver);
    
    private CompareDataBuilder(Func<CompareData, TReceiver> addExpected)
    {
        _addExpected = addExpected;
    }

    public CompareDataBuilder<TReceiver> WithGroups(params string[] groupNames)
    {
        _groupNames = groupNames;

        return this;
    }

    public CompareDataBuilder<TReceiver> AddValueRow(bool[][] values)
    {
        _values.Add(values);

        return this;
    }

    public CompareDataBuilder<TReceiver> AddValueRow(params string[] values)
    {
        bool[][] boolValues = new bool[values.Length][];
        for (int n = 0; n < values.Length; n++)
        {
            var value = values[n];

            if (!value.All(c => c is '0' or '1'))
                throw new ArgumentException($"string value row {value} contains characters that are not 0 or 1");

            var asBools = value.Select(c => c == '1').ToArray();

            boolValues[n] = asBools;
        }

        return AddValueRow(boolValues);
    }

    public TReceiver Build()
    {
        var groups = _groupNames.Select(g => new NamedNodeGroupName(g)).ToArray();
        var values = _values.Select(bvs =>
            bvs.Select(bv => new BitValue(bv)).ToArray()
        ).ToArray();

        var compareData = new CompareData(groups, values);

        return _addExpected(compareData);
    }
}