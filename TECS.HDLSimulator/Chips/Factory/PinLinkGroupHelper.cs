using System;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

public static class PinLinkGroupHelper
{
    public static ArraySegment<INandTreeElement> GetPins(NamedNodeGroup group, PinLinkGroupDescription description)
    {
        var lowerBound = description.LowerBound ?? 0;
        var upperBound = description.UpperBound ?? (group.Nodes.Length - 1);
        var segmentSize = upperBound - lowerBound + 1;

        return new ArraySegment<INandTreeElement>(group.Nodes, lowerBound, segmentSize);
    }
}