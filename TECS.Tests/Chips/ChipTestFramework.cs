using System;
using System.Linq;
using FluentAssertions;
using TECS.DataIntermediates.Names;
using TECS.FileAccess;
using TECS.FileAccess.Mappers;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests.Chips;

public class ChipTestFramework
{
    protected DebugChip TestChip;
    
    private readonly string _chipName;
    private DebugEvaluationResult? _result;
    
    protected ChipTestFramework(string chipName)
    {
        _chipName = chipName;
        
        TestChip = GetChip(_chipName);
        Clock.Instance.Reset();
    }

    protected void RefreshChip()
    {
        TestChip = GetChip(_chipName);
    }
    
    protected string GetInternal(NamedNodeGroupName name) =>
        _result?.InternalValues[name].AsBinaryString() ?? throw new InvalidOperationException("evaluate before getting values");

    protected string GetOutput(NamedNodeGroupName name) =>
        _result?.OutputValues[name].AsBinaryString() ?? throw new InvalidOperationException("evaluate before getting values");

    public enum IncrementMode { None, Single, Double }
    protected void Evaluate(IncrementMode incrementClock = IncrementMode.Double)
    {
        if (incrementClock is IncrementMode.Single or IncrementMode.Double)
        {
            TestChip.IncrementClock();
            TestChip.DebugEvaluate(); // no propagation without evaluation
        }

        if (incrementClock == IncrementMode.Double)
            TestChip.IncrementClock();
        
        _result = TestChip.DebugEvaluate();
    }
    
    private static DebugChip GetChip(string name)
    {
        var dataFolder = new DataFolder(Settings.DataFolder);

        var hdlFolder = dataFolder.HdlFolder;

        var chipData = hdlFolder.HdlFiles.Select(HdlToIntermediateMapper.Map).ToArray();

        var factory = new ChipBlueprintFactory(chipData);

        var debugFac = new DebugChipFactory(factory);

        var result = debugFac.Create(chipData.Single(cd => cd.Name.Value == name));

        DebugChip chip;
        if (result.Success)
        {
            chip = result.Result ?? throw new InvalidOperationException("result was succesful but value was null");
        }
        else
        {
            result.Errors.Should().BeEmpty();
            throw new InvalidOperationException("result was not succesful, but no errors were found");
        }

        return chip;
    }
}