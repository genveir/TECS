namespace TECS.HDLSimulator.Chips.NandTree;

public interface INandTreeNode
{
    // Get the value of this node
    bool[] Value { get; }

    // Clone the node and the entire parent tree
    INandTreeNode Clone(long cloneId);

    // Minimize tree size by fusing all pins upwards. Leaves output and inputs unchanged
    INandTreeNode Fuse(long fuseId);

    // Count how many pins and nands are in this tree
    (int pins, int nands) CountNodes(int countId);
}