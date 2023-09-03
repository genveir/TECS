using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Test;

namespace TECS.DataIntermediates.Builders;

public class CompareDataBuilder<TReceiver>
{
    private readonly Func<CompareData, TReceiver> _addExpected;
    private readonly List<string[]> _values = new();

    private readonly List<ColumnData> _columns = new();

    public static CompareDataBuilder<CompareData> CreateBasic() => new(cpd => cpd);

    public static CompareDataBuilder<TReceiver> 
        WithReceiver(Func<CompareData, TReceiver> receiver) => new(receiver);
    
    private CompareDataBuilder(Func<CompareData, TReceiver> addExpected)
    {
        _addExpected = addExpected;
    }

    public CompareDataBuilder<TReceiver> WithBinaryStringColumns(params string[] data)
    {
        foreach (var datum in data)
        {
            AddColumn(datum, ColumnType.BinaryString);
        }

        return this;
    }

    public CompareDataBuilder<TReceiver> AddColumn(ColumnData columnData)
    {
        _columns.Add(columnData);

        return this;
    }
    
    public CompareDataBuilder<TReceiver> AddColumn(string columnName, ColumnType columnType)
    {
        _columns.Add(new(new(columnName), columnType));

        return this;
    }

    public CompareDataBuilder<TReceiver> AddValueRow(params string[] values)
    {
        _values.Add(values);

        return this;
    }

    public TReceiver Build()
    {
        var groups = _columns.ToArray();
        var values = _values.ToArray();

        var compareData = new CompareData(groups, values);

        return _addExpected(compareData);
    }
}