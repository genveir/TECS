using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;
using TECS.Tests.Builders.ChipData;

namespace TECS.Tests.FileAccess;

public class HdlToIntermediateMapperTests
{
    [Test]
    public void CanMapRegularNot()
    {
        var notHdl = Get("Not");

        var intermediate = HdlToIntermediateMapper.Map(notHdl);

        intermediate.Should().BeEquivalentTo(NotIntermediate);
    }

    [Test]
    public void CanMapRegularAnd()
    {
        var andHdl = Get("And");

        var intermediate = HdlToIntermediateMapper.Map(andHdl);

        intermediate.Should().BeEquivalentTo(AndIntermediate);
    }

    [Test]
    public void CanMapWeirdNot()
    {
        var weirdNotHdl = Get("WeirdNot");

        var intermediate = HdlToIntermediateMapper.Map(weirdNotHdl);

        intermediate.Should().BeEquivalentTo(WeirdNotIntermediate);
    }

    private HdlFile Get(string name)
    {
        var datafolder = new DataFolder(Settings.TestDataFolder);

        var hdlFolder = datafolder.HdlFolder;

        return hdlFolder.GetAllWithName(name).hdl ?? 
               throw new FileNotFoundException($"{name} hdl does not exist but is required for test");
    }

    private static ChipData NotIntermediate => new ChipDataBuilder()
        .WithName("Not")
        .WithInGroups("in")
        .WithOutGroups("out")
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "out")
            .Build()
        .Build();

    private static ChipData AndIntermediate => new ChipDataBuilder()
        .WithName("And")
        .WithInGroups("a", "b")
        .WithOutGroups("out")
        .AddPart("Nand")
            .AddLink("a", "a")
            .AddLink("b", "b")
            .AddLink("out", "mid")
            .Build()
        .AddPart("Not")
            .AddLink("in", "mid")
            .AddLink("out", "out")
            .Build()
        .Build();

    private static ChipData WeirdNotIntermediate => new ChipDataBuilder()
        .WithName("NotWeird")
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