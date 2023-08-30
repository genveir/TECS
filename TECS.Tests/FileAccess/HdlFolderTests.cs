using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;

namespace TECS.Tests.FileAccess;

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

        hdlFiles.Should().HaveCount(17);
        hdlFiles.Any(f => f.Name == "ExtraTest").Should().BeFalse();
        hdlFiles.Any(f => f.Name == "WeirdNot").Should().BeTrue();
    }

    [Test]
    public void CanGetTestFiles()
    {
        IEnumerable<TestFile> testFiles = CreateFixture().HdlFolder.TestFiles;

        testFiles.Should().HaveCount(16);
        testFiles.Any(f => f.Name == "ExtraTest").Should().BeTrue();
        testFiles.Any(f => f.Name == "WeirdNot").Should().BeFalse();
    }

    [Test]
    public void CanGetComparisonFiles()
    {
        IEnumerable<ComparisonFile> comparisonFiles = CreateFixture().HdlFolder.ComparisonFiles;

        comparisonFiles.Should().HaveCount(16);
        comparisonFiles.Any(f => f.Name == "ExtraTest").Should().BeTrue();
        comparisonFiles.Any(f => f.Name == "WeirdNot").Should().BeFalse();
    }

    private DataFolder CreateFixture()
    {
        return new(Settings.TestDataFolder);
    } 
}