using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

internal class PinBoard
{
    public NamedNodeGroupName Name { get; }
    
    public BitSize Size { get; }
    
    public NandPinNode[] Nodes { get; }

    internal INandTreeElement[] CopyNodesToElements()
    {
        var array = new INandTreeElement[Nodes.Length];

        for (int n = 0; n < Nodes.Length; n++)
            array[n] = Nodes[n];

        return array;
    }
    
    public PinBoard(NamedNodeGroupName name, BitSize size)
    {
        Name = name;
        Size = size;

        Nodes = new NandPinNode[size.Value];
        for (int n = 0; n < Nodes.Length; n++)
        {
            Nodes[n] = new NandPinNode();
        }
    }
}