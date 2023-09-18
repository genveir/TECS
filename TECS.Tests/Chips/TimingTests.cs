using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Names;
using TECS.FileAccess;
using TECS.FileAccess.Mappers;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests.Chips;

public class TimingTests
{
    [Test]
    public void CanBuildTestChip()
    {
        BuildTestChip();
    }

    private static readonly NamedNodeGroupName Set = new("set");
    private static readonly NamedNodeGroupName Out = new("out");
    
    [Test]
    public void WhenSetIsFalseOutputAlternates()
    {
        EvaluationResult? result;
        
        var testChip = BuildTestChip();
        
        testChip.SetInput(Set, new(false));
        result = testChip.Evaluate();
        
        result.OutputValues[Out].AsBinaryString().Should().Be("0");
        
        testChip.IncrementClock();
        testChip.Evaluate();
        testChip.IncrementClock();
        result = testChip.Evaluate();

        result.OutputValues[Out].AsBinaryString().Should().Be("1");
        
        testChip.IncrementClock();
        testChip.Evaluate();
        testChip.IncrementClock();
        result = testChip.Evaluate();
        
        result.OutputValues[Out].AsBinaryString().Should().Be("0");
        
        testChip.IncrementClock();
        testChip.Evaluate();
        testChip.IncrementClock();
        result = testChip.Evaluate();
        
        result.OutputValues[Out].AsBinaryString().Should().Be("1");
    }

    private ChipBlueprintFactory? _factory;
    private ChipBlueprintFactory GetBlueprintFactory()
    {
        if (_factory == null)
        {
            var dataFolder = new DataFolder(Settings.DataFolder);

            var hdlFolder = dataFolder.HdlFolder;

            var allChipData = hdlFolder.HdlFiles.Select(HdlToIntermediateMapper.Map).ToArray();

            _factory = new ChipBlueprintFactory(allChipData);
        }

        return _factory;
    }
    
    private DebugChip BuildTestChip()
    {
        var chipData = new ChipDataBuilder()
            .WithName("TimingTest")
            .AddInGroup("set", 1)
            .AddOutGroup("out", 1)
            .AddPart("Not")
                .AddLink("in", "mout")
                .AddLink("out", "flip")
                .Build()
            .AddPart("Mux")
                .AddLink("sel", "set")
                .AddLink("a", "flip")
                .AddLink("b", "true")
                .AddLink("out", "m0")
                .Build()
            .AddPart("Bit")
                .AddLink("load", "true")
                .AddLink("in", "m0")
                .AddLink("out", "mout")
                .Build()
            .AddPart("Pass")
                .AddLink("in", "mout")
                .AddLink("out", "out")
                .Build()
            .Build();

        var factory = GetBlueprintFactory();
        
        return new DebugChipFactory(factory).Create(chipData).Result!;
    }
}