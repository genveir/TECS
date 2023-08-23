using TECS.DataIntermediates.Chip.Names;

namespace TECS.DataIntermediates.Chip;

public class LinkData
{
    public InternalLinkName Internal { get; }
    
    public ExternalLinkName External { get; }

    public LinkData(InternalLinkName internalLinkName, ExternalLinkName externalLinkName)
    {
        Internal = internalLinkName;
        External = externalLinkName;
    }
}