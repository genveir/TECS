using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;

namespace TECS.Tests.Builders.ChipData;

public class ChipPartDataBuilder<TReceiver>
{
    private readonly string _name;
    private readonly List<LinkData> _links = new();

    private readonly Func<ChipPartData, TReceiver> _addPart;

    public static ChipPartDataBuilder<ChipPartData> CreateBasic(string name) => 
        new(name, cpd => cpd);

    public static ChipPartDataBuilder<TReceiver> WithReceiver(string name, Func<ChipPartData, TReceiver> receiver) =>
        new(name, receiver);

    private ChipPartDataBuilder(string name, Func<ChipPartData, TReceiver> addPart)
    {
        _name = name;
        _addPart = addPart;
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
        var part = new ChipPartData(new(_name), _links);

        return _addPart(part);
    }
}