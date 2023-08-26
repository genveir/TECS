using System;
using System.Linq;

namespace TECS.DataIntermediates.Test;

public class TestInputData
{
    public int Order { get; }

    public TestSetData[] SetData { get; }

    internal TestInputData(int order, TestSetData[] setData)
    {
        if (setData.Length != setData.Select(sd => sd.Group).Distinct().Count())
            throw new ArgumentException("test sets same value multiple times");
        
        Order = order;
        SetData = setData;
    }
}