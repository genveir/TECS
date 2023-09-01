using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Chip;

namespace TECS.DataIntermediates.Builders;

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

    public LinkBuilder<ChipPartDataBuilder<TReceiver>> AddLink()
    {
        return LinkBuilder<ChipPartDataBuilder<TReceiver>>.WithReceiver(AddLink);
    }

    public ChipPartDataBuilder<TReceiver> AddLink(string internalName, string externalName) =>
        AddLink()
            .WithInternal(internalName)
            .WithExternal(externalName)
            .Build();

    private ChipPartDataBuilder<TReceiver> AddLink(LinkData linkData)
    {
        _links.Add(linkData);

        return this;
    }

    public TReceiver Build()
    {
        var part = new ChipPartData(new(_name), _links.ToArray());

        return _addPart(part);
    }
}