using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Builders;

public class ChipPartDataBuilder<TReceiver>
{
    private string _name = "";
    private readonly List<LinkData> _links = new();

    private readonly Func<ChipPartData, TReceiver> _addPart;

    public static ChipPartDataBuilder<ChipPartData> CreateBasic() => 
        new(cpd => cpd);

    public static ChipPartDataBuilder<TReceiver> WithReceiver(Func<ChipPartData, TReceiver> receiver) =>
        new(receiver);

    private ChipPartDataBuilder(Func<ChipPartData, TReceiver> addPart)
    {
        _addPart = addPart;
    }

    public ChipPartDataBuilder<TReceiver> WithName(string name)
    {
        _name = name;

        return this;
    }

    public ChipPartDataBuilder<TReceiver> AddLink(string internalName, string externalName)
    {
        var intName = new InternalLinkName(internalName);
        var extName = new ExternalLinkName(externalName);

        var linkData = new LinkData(intName, extName);
        
        _links.Add(linkData);

        return this;
    }

    public TReceiver Build()
    {
        var part = new ChipPartData(new(_name), _links.ToArray());

        return _addPart(part);
    }
}