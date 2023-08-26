using System;
using NUnit.Framework;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;

namespace TECS.Tests.Intermediates.Test;

public class TestDataTests
{
    [Test]
    public void CanCreateTestData()
    {
        _ = new TestDataBuilder()
            .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(AndIntermediate)
                .AddOutput("a", 1)
                .AddOutput("out", 1)
                .SetExpectedValues()
                    .WithGroups("a", "b")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("a", "1")
                    .Build()
                .AddTest(1)
                    .AddInput("a", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CanCreateTestDataWithTestsWithEqualOrder()
    {
        _ = new TestDataBuilder()
            .WithChipToTest(NotIntermediate)
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
            .WithChipToTest(And16Intermediate)
            .AddOutput("a", 16)
            .AddOutput("b", 16)
            .AddOutput("out", 16)
            .SetExpectedValues()
                .WithGroups("a", "b", "out")
                .AddValueRow("0000111100001111", "0000111100001111", "0000111100001111")
                .AddValueRow("0000111100001111", "1111000011110000", "0000000000000000")
                .Build()
            .AddTest(0)
                .AddInput("a", "0000111100001111")
                .AddInput("b", "0000111100001111")
                .Build()
            .AddTest(1)
                .AddInput("a", "0000111100001111")
                .AddInput("b", "1111000011110000")
                .Build()
            .Build();
    }
    
    [Test]
    public void CannotCreateTestDataWithMismatchedBitSizesBetweenOutputAndCompare()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(AndIntermediate)
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
                    .AddInput("a", "0")
                    .AddInput("b", "1")
                    .Build()
                .Build());
    }
    
    [Test]
    public void CannotCreateTestDataWithFewerExpectedValuesThanTests()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(NotIntermediate)
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
                .WithChipToTest(NotIntermediate)
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
            .WithChipToTest(AndIntermediate)
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
            .WithChipToTest(NotIntermediate)
            .AddOutput("out", 1)
            .SetExpectedValues()
                .WithGroups("out")
                .AddValueRow("0")
                .AddValueRow("1")
                .Build()
            .AddTest(0)
                .AddInput("in", "1")
                .Build()
            .AddTest(1)
                .AddInput("in", "0")
                .Build()
            .Build();
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
    
    private static ChipData And16Intermediate => new ChipDataBuilder()
        .WithName("And")
        .WithInGroups("a[16]", "b[16]")
        .WithOutGroups("out[16]")
        .AddPart("Nand16")
            .AddLink("a", "a")
            .AddLink("b", "b")
            .AddLink("out", "mid")
            .Build()
        .AddPart("Not16")
            .AddLink("in", "mid")
            .AddLink("out", "out")
            .Build()
        .Build();
}