using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Test;

public class ColumnData
{
    public NamedNodeGroupName Name { get; }
    
    public ColumnType Type { get; }

    public ColumnData(NamedNodeGroupName name, ColumnType type)
    {
        Name = name;
        Type = type;
    }
}