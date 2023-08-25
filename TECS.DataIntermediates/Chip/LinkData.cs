using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Chip;

public class LinkData
{
    public InternalLinkName Internal { get; }
    
    public ExternalLinkName External { get; }

    internal LinkData(InternalLinkName internalLinkName, ExternalLinkName externalLinkName)
    {
        Internal = internalLinkName;
        External = externalLinkName;
    }
}