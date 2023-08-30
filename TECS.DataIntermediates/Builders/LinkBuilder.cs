using System;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Builders;

public class LinkBuilder<TReceiver>
{
    private readonly Func<LinkData, TReceiver> _addLink;

    private InternalLinkData? _internal;
    private ExternalLinkData? _external;

    public static LinkBuilder<LinkData> CreateBasic() => 
        new(ld => ld);

    public static LinkBuilder<TReceiver> WithReceiver(Func<LinkData, TReceiver> receiver) =>
        new(receiver);

    private LinkBuilder(Func<LinkData, TReceiver> addLink)
    {
        _addLink = addLink;
    }

    public LinkBuilder<TReceiver> WithInternal(string name, int? lowerBound = null, int? upperBound = null)
    {
        var linkName = new InternalLinkName(name);
        _internal = new(linkName, lowerBound, upperBound);
        
        return this;
    }

    public LinkBuilder<TReceiver> WithExternal(string name, int? lowerBound = null, int? upperBound = null)
    {
        var linkName = new ExternalLinkName(name);
        _external = new(linkName, lowerBound, upperBound);

        return this;
    }

    public TReceiver Build()
    {
        if (_internal == null)
            throw new BuilderException("internal part of the link can not be empty");
        if (_external == null)
            throw new BuilderException("external part of the link can not be empty");
        
        var linkData = new LinkData(_internal, _external);

        return _addLink(linkData);
    }
}