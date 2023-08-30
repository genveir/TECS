using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Chip;

public class InternalLinkData
{
    public InternalLinkName Name { get; }
    
    public int? LowerPin { get; }
    
    public int? UpperPin { get; }

    public InternalLinkData(InternalLinkName name, int? lowerPin, int? upperPin)
    {
        Name = name;
        LowerPin = lowerPin;
        UpperPin = upperPin;
    }
}