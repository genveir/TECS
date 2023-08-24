using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Chip.Names;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;

namespace TECS.Tests.FileAccess;

public class HdlToIntermediateMapperTests
{
    [Test]
    public void CanParseRegularNot()
    {
        var notHdl = Get("Not");

        var intermediate = HdlToIntermediateMapper.Map(notHdl);

        intermediate.Should().BeEquivalentTo(ParsedNotIntermediate);
    }

    [Test]
    public void CanParseWeirdNot()
    {
        var weirdNotHdl = Get("WeirdNot");

        var intermediate = HdlToIntermediateMapper.Map(weirdNotHdl);

        intermediate.Should().BeEquivalentTo(ParsedWeirdNotIntermediate);
    }

    private HdlFile Get(string name)
    {
        var datafolder = new DataFolder(Settings.TestDataFolder);

        var hdlFolder = datafolder.HdlFolder;

        return hdlFolder.GetAllWithName(name).hdl ?? 
               throw new FileNotFoundException($"{name} hdl does not exist but is required for test");
    }
    
    private static ChipData ParsedNotIntermediate => new(
        name: new("Not"),
        inGroups: new List<NamedNodeGroupName> { new("in") },
        outGroups: new List<NamedNodeGroupName> { new("out") },
        parts: new List<ChipPartData>
        {
            new(
                partName: new ChipName("Nand"),
                links: new List<LinkData>()
                {
                    new(new("a"), new("in")),
                    new(new("b"), new("in")),
                    new(new("out"), new("out"))
                })
        });
    
    private static ChipData ParsedWeirdNotIntermediate => new(
        name: new("Not"),
        inGroups: new List<NamedNodeGroupName> { new("in") },
        outGroups: new List<NamedNodeGroupName> { new("out"), new("out2") },
        parts: new List<ChipPartData>
        {
            new(
                partName: new ChipName("Nand"),
                links: new List<LinkData>()
                {
                    new(new("a"), new("in")),
                    new(new("b"), new("in")),
                    new(new("out"), new("out")),
                    new(new("out"), new("out2")) // this is illegal, but not at this level
                })
        });
}