using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.DataIntermediates.Chip;

public class NamedNodeGroupData
{
    public NamedNodeGroupName Name { get; }
    
    public BitSize Size { get; }

    public NamedNodeGroupData(NamedNodeGroupName name, BitSize size)
    {
        Name = name;
        Size = size;
    }
}