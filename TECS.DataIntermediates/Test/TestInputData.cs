namespace TECS.DataIntermediates.Test;

public class TestInputData
{
    public int Order { get; }

    public TestSetData[] SetData { get; }

    internal TestInputData(int order, TestSetData[] setData)
    {
        Order = order;
        SetData = setData;
    }
}