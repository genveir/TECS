using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Chip.Names;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;
using TECS.Tests.Builders.ChipData;

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

    private static ChipData ParsedNotIntermediate => new ChipDataBuilder()
        .WithName("Not")
        .WithInGroups("in")
        .WithOutGroups("out")
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "out")
            .Build()
        .Build();

    private static ChipData ParsedWeirdNotIntermediate => new ChipDataBuilder()
        .WithName("Not")
        .WithInGroups("in")
        .WithOutGroups("out", "out2")
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "out")
            .AddLink("out", "out2")
            .Build()
        .Build();
}