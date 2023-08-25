using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Test;

public class OutputListData
{
    public NamedNodeGroupName Group { get; }
    
    public BitSize BitSize { get; }

    public OutputListData(NamedNodeGroupName group, BitSize bitSize)
    {
        Group = group;
        BitSize = bitSize;
    }
}