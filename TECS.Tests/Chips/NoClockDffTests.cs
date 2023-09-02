using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests.Chips;

public class NoClockDffTests
{
    [Test]
    public void CanFabricateNoClockDff()
    {
        Assert.DoesNotThrow(() => _ = GetNoClockDff());
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CanEvaluateNoClockDff(bool input)
    {
        var dff = GetNoClockDff();

        var inName = new NamedNodeGroupName("in");
        var outName = new NamedNodeGroupName("out");
        
        dff.SetInput(inName, input ? BitValue.True : BitValue.False);

        var result = dff.Evaluate();

        result.OutputValues[outName].Should().Be(input ? BitValue.False : BitValue.True);
    }

    private Chip GetNoClockDff()
    {
        var factory = new ChipBlueprintFactory(new List<ChipData> { _noClockDff });

        var factoryResult = factory.GetBlueprint(new("NoClockDff"));

        factoryResult.Errors.Should().BeEmpty();
        factoryResult.Success.Should().BeTrue();

        var blueprint = factoryResult.Result;
        
        return blueprint?.Fabricate() ?? throw new InvalidOperationException("huh");
    }
    
    private readonly ChipData _noClockDff = new ChipDataBuilder()
        .WithName("NoClockDff")
        .AddInGroup("in", 1)
        .AddOutGroup("out", 1)
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "in")
            .AddLink("out", "nin")
            .Build()
        .AddPart("Nand")
            .AddLink("a", "in")
            .AddLink("b", "q0")
            .AddLink("out", "q")
            .Build()
        .AddPart("Nand")
            .AddLink("a", "q")
            .AddLink("b", "nin")
            .AddLink("out", "q0")
            .Build()
        .AddPart("Pass")
            .AddLink("in", "q")
            .AddLink("out", "out")
            .Build()
        .Build();
}