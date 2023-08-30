namespace TECS.DataIntermediates.Chip;

public class LinkData
{
    public InternalLinkData Internal { get; }
    
    public ExternalLinkData External { get; }

    internal LinkData(InternalLinkData internalLinkName, ExternalLinkData externalLinkName)
    {
        Internal = internalLinkName;
        External = externalLinkName;
    }
}