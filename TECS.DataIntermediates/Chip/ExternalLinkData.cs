using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Chip;

public class ExternalLinkData
{
    public ExternalLinkName Name { get; }
    
    public int? LowerPin { get; }
    
    public int? UpperPin { get; }

    public ExternalLinkData(ExternalLinkName name, int? lowerPin, int? upperPin)
    {
        Name = name;
        LowerPin = lowerPin;
        UpperPin = upperPin;
    }
}