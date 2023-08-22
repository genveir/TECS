using System.Collections.Generic;

namespace TECS.HDLSimulator.HDL;

public class ChipDescription
{
    public string Name { get; }
    public List<NamedPinGroupDescription> In { get; }
    public List<NamedPinGroupDescription> Out { get; }
    public List<PartDescription> Parts { get; }

    public ChipDescription(string name, List<NamedPinGroupDescription> pinsIn, List<NamedPinGroupDescription> pinsOut, List<PartDescription> parts)
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

public class NamedPinGroupDescription
{
    public string Name { get; }
    public int BitSize { get; }

    public NamedPinGroupDescription(string name, int bitSize)
    {
        Name = name;
        BitSize = bitSize;
    }

    public override string ToString()
    {
        return $"NamedPinGroupDescription {Name}[{BitSize}]";
    }
}

public class PartDescription
{
    public string Name { get; }
    public readonly List<PinConnectionDescription> PinConnections;

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
    public PinLinkGroupDescription Local { get; }
    public PinLinkGroupDescription External { get; }

    public PinConnectionDescription(PinLinkGroupDescription local, PinLinkGroupDescription external)
    {
        Local = local;
        External = external;
    }

    public override string ToString()
    {
        return $"PinConnectionDescription {Local}={External}";
    }
}

public class PinLinkGroupDescription
{
    public string Name { get; }
    public int? LowerBound { get; }
    public int? UpperBound { get; }

    public PinLinkGroupDescription(string name, int? lowerBound, int? upperBound)
    {
        Name = name;
        LowerBound = lowerBound;
        UpperBound = upperBound;
    }

    public override string ToString()
    {
        var lowerbound = LowerBound?.ToString() ?? "_";
        var upperbound = UpperBound?.ToString() ?? "_";
        
        return $"PinLinkGroupDescription {Name}[{lowerbound}..{upperbound}]";
    }
}