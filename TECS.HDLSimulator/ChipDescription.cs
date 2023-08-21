using System.Collections.Generic;

namespace TECS.HDLSimulator;

public class ChipDescription
{
    public string Name { get; }
    public List<PinDescription> In { get; }
    public List<PinDescription> Out { get; }
    public List<PartDescription> Parts { get; }

    public ChipDescription(string name, List<PinDescription> pinsIn, List<PinDescription> pinsOut, List<PartDescription> parts)
    {
        Name = name;
        In = pinsIn;
        Out = pinsOut;
        Parts = parts;
    }

    public override string ToString()
    {
        return $"ChipDescription {Name}";
    }
}

public class PinDescription
{
    public string Name { get; }
    public int BitSize { get; }

    public PinDescription(string name, int bitSize)
    {
        Name = name;
        BitSize = bitSize;
    }

    public override string ToString()
    {
        return $"PinDescription {Name}[{BitSize}]";
    }
}

public class PartDescription
{
    public string Name { get; }
    public List<PinConnectionDescription> PinConnections;

    public PartDescription(List<PinConnectionDescription> pinConnections, string name)
    {
        PinConnections = pinConnections;
        Name = name;
    }

    public override string ToString()
    {
        return $"PartDescription {Name}";
    }
}

public class PinConnectionDescription
{
    public string Local { get; }
    public string External { get; }

    public PinConnectionDescription(string local, string external)
    {
        Local = local;
        External = external;
    }

    public override string ToString()
    {
        return $"PinConnectionDescription {Local}={External}";
    }
}