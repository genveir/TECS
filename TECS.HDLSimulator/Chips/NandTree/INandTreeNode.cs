namespace TECS.HDLSimulator.Chips.NandTree;

public interface INandTreeNode
{
    bool[] Value { get; }

    INandTreeNode Clone(long cloneId);

    INandTreeNode Fuse(long fuseId);

    (int pins, int nands) CountNodes(int countId);
}