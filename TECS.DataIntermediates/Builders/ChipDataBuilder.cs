using System.Collections.Generic;
using TECS.DataIntermediates.Chip;

namespace TECS.DataIntermediates.Builders;

public class ChipDataBuilder
{
    private string _name = "defaultName";
    private readonly List<NamedNodeGroupData> _inGroups = new(); 
    private readonly List<NamedNodeGroupData> _outGroups = new();
    private readonly List<ChipPartData> _parts = new();

    public ChipDataBuilder WithName(string name)
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

    public ChipDataBuilder AddOutGroup(string name, int bitSize)
    {
        var nodeGroupData = NodeGroupDataFromNameAndSize(name, bitSize);
        
        _outGroups.Add(nodeGroupData);
        
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

    public ChipData Build()
    {
        return new(
            name: new(_name),
            inGroups: _inGroups.ToArray(),
            outGroups: _outGroups.ToArray(),
            _parts.ToArray()
        );
    }
}