using System.Collections.Generic;

namespace TECS.HDLSimulator;

public class ChipSummary
{
    public string Name { get; }
    public List<PinSummary> In { get; }
    public List<PinSummary> Out { get; }
    public List<PartSummary> Parts { get; }

    public ChipSummary(string name, List<PinSummary> pinsIn, List<PinSummary> pinsOut, List<PartSummary> parts)
    {
        Name = name;
        In = pinsIn;
        Out = pinsOut;
        Parts = parts;
    }
}

public class PinSummary
{
    public string Name { get; }
    public int BitSize { get; }

    public PinSummary(string name, int bitSize)
    {
        Name = name;
        BitSize = bitSize;
    }
}

public class PartSummary
{
    public string Name { get; }
    public List<PinConnectionSummary> PinConnections;

    public PartSummary(List<PinConnectionSummary> pinConnections, string name)
    {
        PinConnections = pinConnections;
        Name = name;
    }
}

public class PinConnectionSummary
{
    public string Local { get; }
    public string External { get; }

    public PinConnectionSummary(string local, string external)
    {
        Local = local;
        External = external;
    }
}