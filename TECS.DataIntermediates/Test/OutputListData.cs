using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Test;

public class OutputListData
{
    public NamedNodeGroupName Group { get; }
    
    public BitSize BitSize { get; }

    internal OutputListData(NamedNodeGroupName group, BitSize bitSize)
    {
        Group = group;
        BitSize = bitSize;
    }
}