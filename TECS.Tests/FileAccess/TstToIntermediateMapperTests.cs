using System.IO;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Test;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;

namespace TECS.Tests.FileAccess;

public class TstToIntermediateMapperTests
{
    [Test]
    public void CanTestRegularNot()
    {
        var (hdlFolder, testFile) = Get("Not");

        var intermediate = TstToIntermediateMapper.Map(hdlFolder, testFile);

        intermediate.Should().BeEquivalentTo(NotIntermediate);
    }

    [Test]
    public void CanMapExtraTest()
    {
        var (hdlFolder, testFile) = Get("ExtraTest");

        var intermediate = TstToIntermediateMapper.Map(hdlFolder, testFile);

        intermediate.Should().BeEquivalentTo(NotIntermediate);
    }

    [Test]
    public void CanMapRegularAnd()
    {
        var (hdlFolder, testFile) = Get("And");

        var intermediate = TstToIntermediateMapper.Map(hdlFolder, testFile);

        intermediate.Should().BeEquivalentTo(AndIntermediate);
    }
    
    [Test]
    public void CanMap16BitAnd()
    {
        var (hdlFolder, testFile) = Get("And16");

        var intermediate = TstToIntermediateMapper.Map(hdlFolder, testFile);

        intermediate.Should().BeEquivalentTo(And16Intermediate);
    }
    
    private (HdlFolder, TestFile) Get(string name)
    {
        var datafolder = new DataFolder(Settings.TestDataFolder);

        var hdlFolder = datafolder.HdlFolder;

        var testFile = hdlFolder.GetAllWithName(name).tst ?? 
               throw new FileNotFoundException($"{name} hdl does not exist but is required for test");

        return (hdlFolder, testFile);
    }

    private TestData NotIntermediate => HandMadeIntermediates.NotTestIntermediate;
    private TestData AndIntermediate => HandMadeIntermediates.AndTestIntermediate;
    private TestData And16Intermediate => HandMadeIntermediates.And16TestIntermediate;
}