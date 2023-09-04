using System.Collections.Generic;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;

namespace TECS.DataIntermediates.Builders;

public class ChipDataBuilder
{
    private ChipName _name = new("defaultName");
    private readonly List<NamedNodeGroupData> _inGroups = new(); 
    private readonly List<NamedNodeGroupData> _outGroups = new();
    private readonly List<ChipPartData> _parts = new();

    public ChipDataBuilder WithName(string name)
    {
        _name = new(name);

        return this;
    }

    public ChipDataBuilder WithName(ChipName name)
    {
        _name = name;

        return this;
    }

    public ChipDataBuilder AddInGroup(string name, int bitSize)
    {
        var nodeGroupData = NodeGroupDataFromNameAndSize(name, bitSize);
        
        _inGroups.Add(nodeGroupData);
        
        return this;
    }

    public ChipDataBuilder WithInGroups(IEnumerable<NamedNodeGroupData> inGroups)
    {
        _inGroups.Clear();
        _inGroups.AddRange(inGroups);

        return this;
    }

    public ChipDataBuilder AddOutGroup(string name, int bitSize)
    {
        var nodeGroupData = NodeGroupDataFromNameAndSize(name, bitSize);
        
        _outGroups.Add(nodeGroupData);
        
        return this;
    }
    
    public ChipDataBuilder WithOutGroups(IEnumerable<NamedNodeGroupData> outGroups)
    {
        _outGroups.Clear();
        _outGroups.AddRange(outGroups);

        return this;
    }

    private static NamedNodeGroupData NodeGroupDataFromNameAndSize(string name, int size) =>
        new(new(name), new(size));

    public ChipPartDataBuilder<ChipDataBuilder> AddPart(string name)
    {
        return ChipPartDataBuilder<ChipDataBuilder>
            .WithReceiver(name, AddPart);
    }

    private ChipDataBuilder AddPart(ChipPartData chipPartData)
    {
        _parts.Add(chipPartData);

        return this;
    }

    public ChipDataBuilder WithParts(IEnumerable<ChipPartData> parts)
    {
        _parts.Clear();
        _parts.AddRange(parts);

        return this;
    }

    public ChipData Build()
    {
        return new(
            name: _name,
            inGroups: _inGroups.ToArray(),
            outGroups: _outGroups.ToArray(),
            parts: _parts.ToArray()
        );
    }
}