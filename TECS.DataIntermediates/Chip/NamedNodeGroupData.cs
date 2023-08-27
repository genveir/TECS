using TECS.DataIntermediates.Names;

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