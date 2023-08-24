using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Chip.Names;

namespace TECS.Tests.Builders.ChipData;

public class ChipDataBuilder
{
    private string _name = "defaultName";
    private string[] _inGroups = Array.Empty<string>(); 
    private string[] _outGroups = Array.Empty<string>();
    private List<ChipPartData> _parts = new(); 
    
    public ChipDataBuilder WithName(string name)
    {
        _name = name;

        return this;
    }

    public ChipDataBuilder WithInGroups(params string[] names)
    {
        _inGroups = names;

        return this;
    }

    public ChipDataBuilder WithOutGroups(params string[] names)
    {
        _outGroups = names;

        return this;
    }

    public ChipPartDataBuilder<ChipDataBuilder> AddPart(string name)
    {
        return new ChipPartDataBuilder<ChipDataBuilder>(name, AddPart);
    }

    public ChipDataBuilder AddPart(ChipPartData chipPartData)
    {
        _parts.Add(chipPartData);

        return this;
    }

    public DataIntermediates.Chip.ChipData Build()
    {
        var inGroups = _inGroups.Select(s => new NamedNodeGroupName(s));
        var outGroups = _outGroups.Select(s => new NamedNodeGroupName(s));
        
        return new(
            name: new(_name),
            inGroups: inGroups,
            outGroups: outGroups,
            _parts
        );
    }
}