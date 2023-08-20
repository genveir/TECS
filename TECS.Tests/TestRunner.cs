using FluentAssertions;
using NUnit.Framework;
using TECS.HDLSimulator.Chips;

namespace TECS.Tests;

public class TestRunner
{
    [TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.Create), new object?[] {Settings.DataFolder})]
    public void RunTest(string name, Chip chip, TestFile testFile, ComparisonFile comparisonFile)
    {
        name.Should().NotBeNull();
        chip.Should().NotBeNull();
        testFile.Should().NotBeNull();
        comparisonFile.Should().NotBeNull();
    }
}