using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;

namespace TECS.Tests.Parser;

public class HdlFolderTests
{
    [Test]
    public void CanGetHdlFolder()
    {
        HdlFolder folder = CreateFixture().HdlFolder;

        folder.Should().NotBeNull();
    }

    [Test]
    public void CanGetHdlFiles()
    {
        IEnumerable<HdlFile> hdlFiles = CreateFixture().HdlFolder.HdlFiles;

        hdlFiles.Should().HaveCount(15);
        hdlFiles.Any(f => f.ToString().Contains("ExtraTest")).Should().BeFalse();
    }

    [Test]
    public void CanGetTestFiles()
    {
        IEnumerable<TestFile> testFiles = CreateFixture().HdlFolder.TestFiles;

        testFiles.Should().HaveCount(16);
        testFiles.Any(f => f.Name == "ExtraTest").Should().BeTrue();
    }

    [Test]
    public void CanGetComparisonFiles()
    {
        IEnumerable<ComparisonFile> comparisonFiles = CreateFixture().HdlFolder.ComparisonFiles;

        comparisonFiles.Should().HaveCount(16);
        comparisonFiles.Any(f => f.Name == "ExtraTest").Should().BeTrue();
    }

    private DataFolder CreateFixture()
    {
        return new(Settings.TestDataFolder);
    } 
}