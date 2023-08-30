using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;

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
    public void CanMap16BitAnd()
    {
        var and16Hdl = Get("And16");

        var intermediate = HdlToIntermediateMapper.Map(and16Hdl);

        intermediate.Should().BeEquivalentTo(And16Intermediate);
    }

    [Test]
    public void CanMapWeirdNot()
    {
        var weirdNotHdl = Get("WeirdNot");

        var intermediate = HdlToIntermediateMapper.Map(weirdNotHdl);

        intermediate.Should().BeEquivalentTo(WeirdNotIntermediate);
    }

    [Test]
    public void CanMapLinkTest()
    {
        var linkTestHdl = Get("LinkTest");

        var intermediate = HdlToIntermediateMapper.Map(linkTestHdl);

        intermediate.Should().BeEquivalentTo(LinkTestIntermediate);
    }

    private HdlFile Get(string name)
    {
        var datafolder = new DataFolder(Settings.TestDataFolder);

        var hdlFolder = datafolder.HdlFolder;

        return hdlFolder.GetAllWithName(name).hdl ?? 
               throw new FileNotFoundException($"{name} hdl does not exist but is required for test");
    }

    private static ChipData NotIntermediate => HandMadeIntermediates.NotIntermediate;
    private static ChipData AndIntermediate => HandMadeIntermediates.AndIntermediate;
    private static ChipData And16Intermediate => HandMadeIntermediates.And16Intermediate;
    private static ChipData WeirdNotIntermediate => HandMadeIntermediates.WeirdNotIntermediate;
    private static ChipData LinkTestIntermediate => HandMadeIntermediates.LinkTestIntermediate;
}