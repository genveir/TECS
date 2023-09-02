using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Test;

public class TestSetData
{
    public NamedNodeGroupName Group { get; }
    
    public BitValue ValueToSet { get; }

    internal TestSetData(NamedNodeGroupName group, BitValue valueToSet)
    {
        Group = group;
        ValueToSet = valueToSet;
    }
}