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
            .WithExpectedValues()
                .WithBinaryStringColumns("in", "out")
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
    public void CannotCreateTestDataWithEmptyTestSet()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest(NotIntermediate)
                .WithExpectedValues()
                    .WithBinaryStringColumns("in", "out")
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
                .WithExpectedValues()
                    .WithBinaryStringColumns("in", "out")
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
    public void CannotCreateTestDataWithTestsWithEqualOrder()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest(NotIntermediate)
                .WithExpectedValues()
                    .WithBinaryStringColumns("in", "out")
                    .AddValueRow("1", "0")
                    .AddValueRow("0", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "1")
                    .Build()
                .AddTest(0)
                    .AddInput("in", "0")
                    .Build()
                .Build());
    }

    [Test]
    public void CanCreateTestDataWithLargerBitSizes()
    {
        _ = new TestDataBuilder()
            .WithChipToTest(And16Intermediate)
            .WithExpectedValues()
                .WithBinaryStringColumns("a", "b", "out")
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
    public void CannotCreateTestDataWithMoreExpectedValuesThanTests()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new TestDataBuilder()
                .WithChipToTest(AndIntermediate)
                .WithExpectedValues()
                    .WithBinaryStringColumns("a", "b", "out")
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
                .WithExpectedValues()
                    .WithBinaryStringColumns("in", "out")
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
                .WithExpectedValues()
                    .WithBinaryStringColumns("in", "out")
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
    public void CanCreateTestWhereNotEveryValueIsSetEveryTest()
    {
        _ = new TestDataBuilder()
            .WithChipToTest(AndIntermediate)
            .WithExpectedValues()
                .WithBinaryStringColumns("a", "b", "out")
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
            .WithExpectedValues()
                .WithBinaryStringColumns("out")
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

    [Test]
    public void CanCreateTestDataThatDoesNotMatchChip()
    {
        _ = new TestDataBuilder()
            .WithChipToTest(And16Intermediate)
            .WithExpectedValues()
                .WithBinaryStringColumns("in", "out")
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
    
    private static ChipData NotIntermediate => new ChipDataBuilder()
        .WithName("Not")
        .AddInGroup("in", 1)
        .AddOutGroup("out", 1)
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "out")
            .Build()
        .Build();
    
    private static ChipData AndIntermediate => new ChipDataBuilder()
        .WithName("And")
        .AddInGroup("a", 1)
        .AddInGroup("b", 1)
        .AddOutGroup("out", 1)
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
        .AddInGroup("a", 16)
        .AddInGroup("b", 16)
        .AddOutGroup("out", 16)
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