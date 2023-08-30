using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Test;

namespace TECS.Tests.FileAccess;

internal static class HandMadeIntermediates
{
    public static TestData NotTestIntermediate => new TestDataBuilder()
        .WithChipToTest(NotIntermediate)
        .AddOutput("in", 1)
        .AddOutput("out", 1)
        .SetExpectedValues()
            .WithGroups("in", "out")
            .AddValueRow("0", "1")
            .AddValueRow("1", "0")
            .Build()
        .AddTest(0)
            .AddInput("in", "0")
            .Build()
        .AddTest(1)
            .AddInput("in", "1")
            .Build()
        .Build();

    public static ChipData NotIntermediate => new ChipDataBuilder()
        .WithName("Not")
        .AddInGroup("in", 1)
        .AddOutGroup("out", 1)
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "out")
            .Build()
        .Build();

    public static TestData AndTestIntermediate => new TestDataBuilder()
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
        .AddTest(2)
            .AddInput("a", "1")
            .AddInput("b", "0")
            .Build()
        .AddTest(3)
            .AddInput("a", "1")
            .AddInput("b", "1")
            .Build()
        .Build();

    public static ChipData AndIntermediate => new ChipDataBuilder()
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

    public static ChipData WeirdNotIntermediate => new ChipDataBuilder()
        .WithName("NotWeird")
        .AddInGroup("in", 1)
        .AddOutGroup("out", 1)
        .AddOutGroup("out2", 1)
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "out")
            .AddLink("out", "out2")
            .Build()
        .Build();

    public static TestData And16TestIntermediate => new TestDataBuilder()
        .WithChipToTest(And16Intermediate)
        .AddOutput("a", 16)
        .AddOutput("b", 16)
        .AddOutput("out", 16)
        .SetExpectedValues()
        .WithGroups("a", "b", "out")
            .AddValueRow("0000000000000000", "0000000000000000", "0000000000000000")
            .AddValueRow("0000000000000000", "1111111111111111", "0000000000000000")
            .AddValueRow("1111111111111111", "1111111111111111", "1111111111111111")
            .AddValueRow("1010101010101010", "0101010101010101", "0000000000000000")
            .AddValueRow("0011110011000011", "0000111111110000", "0000110011000000")
            .AddValueRow("0001001000110100", "1001100001110110", "0001000000110100")
            .Build()
        .AddTest(0)
            .AddInput("a", "0000000000000000")
            .AddInput("b", "0000000000000000")
            .Build()
        .AddTest(1)
            .AddInput("a", "0000000000000000")
            .AddInput("b", "1111111111111111")
            .Build()
        .AddTest(2)
            .AddInput("a", "1111111111111111")
            .AddInput("b", "1111111111111111")
            .Build()
        .AddTest(3)
            .AddInput("a", "1010101010101010")
            .AddInput("b", "0101010101010101")
            .Build()
        .AddTest(4)
            .AddInput("a", "0011110011000011")
            .AddInput("b", "0000111111110000")
            .Build()
        .AddTest(5)
            .AddInput("a", "0001001000110100")
            .AddInput("b", "1001100001110110")
            .Build()
        .Build();

    public static ChipData And16Intermediate
    {
        get
        {
            var builder = new ChipDataBuilder()
                .WithName("And16")
                .AddInGroup("a", 16)
                .AddInGroup("b", 16)
                .AddOutGroup("out", 16);

            for (int n = 0; n < 16; n++)
            {
                builder.AddPart("And")
                    .AddLink()
                        .WithInternal("a")
                        .WithExternal("a", n, n)
                        .Build()
                    .AddLink()
                        .WithInternal("b")
                        .WithExternal("b", n, n)
                        .Build()
                    .AddLink()
                        .WithInternal("out")
                        .WithExternal("out", n, n)
                        .Build()
                    .Build();
            }

            return builder.Build();
        }
    }

    public static ChipData LinkTestIntermediate =>new ChipDataBuilder()
        .WithName("LinkTest")
        .AddInGroup("a", 16)
        .AddOutGroup("out", 16)
        .AddPart("Simple")
            .AddLink()
                .WithInternal("in", null, null)
                .WithExternal("a", null, null)
                .Build()
            .AddLink()
                .WithInternal("ex", null, null)
                .WithExternal("out", null, null)
                .Build()
            .Build()
        .AddPart("SpecifiedPin")
            .AddLink()
                .WithInternal("in", 1, 1)
                .WithExternal("a", 5,5)
                .Build()
            .AddLink()
                .WithInternal("ex", 10, 10)
                .WithExternal("out", 5, 5)
                .Build()
            .Build()
        .AddPart("SpecifiedRange")
            .AddLink()
                .WithInternal("in", 1, 5)
                .WithExternal("a", 5, 9)
                .Build()
            .AddLink()
                .WithInternal("ex", 3, 10)
                .WithExternal("out", 5, 12)
                .Build()
            .Build()
        .AddPart("TrueFalse")
            .AddLink()
                .WithInternal("q", null, null)
                .WithExternal("true", null, null)
                .Build()
            .AddLink()
                .WithInternal("p", 0, 2)
                .WithExternal("false", null, null)
                .Build()
            .AddLink()
                .WithInternal("ex", null, null)
                .WithExternal("out", null, null)
                .Build()
            .Build()
        .Build();
}