namespace TECS.DataIntermediates.Test;

public class TestInputData
{
    public int Order { get; }

    public TestSetData[] SetData { get; }

    public TestInputData(int order, TestSetData[] setData)
    {
        Order = order;
        SetData = setData;
    }
}