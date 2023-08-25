using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Test;

public class TestSetData
{
    public NamedNodeGroupName Group { get; }
    
    public BitValue ValueToSet { get; }

    public TestSetData(NamedNodeGroupName group, BitValue valueToSet)
    {
        Group = group;
        ValueToSet = valueToSet;
    }
}