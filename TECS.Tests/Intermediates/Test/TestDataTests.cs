using System;
using NUnit.Framework;
using TECS.DataIntermediates.Builders;

namespace TECS.Tests.Intermediates.Test;

public class TestDataTests
{
    [Test]
    public void CanCreateTestData()
    {
        _ = new TestDataBuilder()
            .WithChipToTest("NotChip")
            .AddOutput("in", 1)
            .AddOutput("out", 1)
            .SetExpectedValues()
                .WithGroups("in", "out")
                .AddValueRow("1", "0")
                .AddValueRow("0", "1")
                .Build()
            .AddTest(0)
                .AddInput("in", "1")
                .Build()
            .AddTest(1)
                .AddInput("in", "0")
                .Build()
            .Build();
    }

    [Test]
    public void CannotCreateTestDataWithEmptyOutputList()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CannotCreateTestDataWithEmptyTestSet()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .Build());
    }

    [Test]
    public void CannotCreateTestDataWithDoubleGroupInOutputList()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                    .AddOutput("in", 1)
                    .AddOutput("in", 16)
                    .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "0")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CannotCreateTestDataWithMismatchedOutputListAndCompareColumns()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "x")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CanCreateTestDataWithTestsWithEqualOrder()
    {
        _ = new TestDataBuilder()
            .WithChipToTest("NotChip")
            .AddOutput("in", 1)
            .AddOutput("out", 1)
            .SetExpectedValues()
                .WithGroups("in", "out")
                .AddValueRow("1", "0")
                .AddValueRow("0", "1")
                .Build()
            .AddTest(0)
                .AddInput("in", "1")
                .Build()
            .AddTest(0)
                .AddInput("in", "0")
                .Build()
            .Build();
    }

    [Test]
    public void CanCreateTestDataWithLargerBitSizes()
    {
        _ = new TestDataBuilder()
            .WithChipToTest("NotChip")
            .AddOutput("in", 2)
            .AddOutput("out", 16)
            .SetExpectedValues()
                .WithGroups("in", "out")
                .AddValueRow("10", "0000111100001111")
                .AddValueRow("01", "1111000011110000")
                .Build()
            .AddTest(0)
                .AddInput("in", "10")
                .Build()
            .AddTest(1)
                .AddInput("in", "01")
                .Build()
            .Build();
    }
    
    [Test]
    public void CannotCreateTestDataWithMismatchedBitSizesBetweenOutputAndCompare()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "01")
                    .AddValueRow("0", "10")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CannotCreateTestDataWithMoreExpectedValuesThanTests()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 2)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("00", "0")
                    .AddValueRow("01", "1")
                    .AddValueRow("10", "1")
                    .AddValueRow("11", "1")
                .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }
    
    [Test]
    public void CannotCreateTestDataWithFewerExpectedValuesThanTests()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "0")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CannotCreateTestDataWhichSetsTheSameValueTwiceInOneTest()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .AddInput("in", "0")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CannotSetATestInputToTheWrongBitSize()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest("NotChip")
                .AddOutput("in", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("in", "out")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "0000111100001111")
                    .Build()
                .AddTest(1)
                    .AddInput("in", "1111000011110000")
                    .Build()
                .Build());
    }

    [Test]
    public void CanCreateTestWhereNotEveryValueIsSetEveryTest()
    {
        _ = new TestDataBuilder()
            .WithChipToTest("AndChip")
            .AddOutput("a", 1)
            .AddOutput("b", 1)
            .AddOutput("out", 1)
            .SetExpectedValues()
                .WithGroups("a", "b", "out")
                .AddValueRow("0", "0", "0")
                .AddValueRow("0", "1", "0")
                .AddValueRow("1", "0", "0")
                .AddValueRow("1", "1", "1")
                .Build()
            .AddTest(0)
                .AddInput("a", "0")
                .AddInput("b", "0")
                .Build()
            .AddTest(1)
                .AddInput("b", "1")
                .Build()
            .AddTest(2)
                .AddInput("a", "1")
                .AddInput("b", "0")
                .Build()
            .AddTest(3)
                .AddInput("b", "1")
                .Build()
            .Build();
    }

    [Test]
    public void CanSetInputThatIsNotInOutputList()
    {
        _ = new TestDataBuilder()
            .WithChipToTest("NotChip")
            .AddOutput("out", 1)
            .SetExpectedValues()
                .WithGroups("out")
                .AddValueRow("0")
                .AddValueRow("1")
                .Build()
            .AddTest(0)
                .AddInput("in", "1010")
                .Build()
            .AddTest(1)
                .AddInput("in", "0")
                .Build()
            .Build();
    }
}