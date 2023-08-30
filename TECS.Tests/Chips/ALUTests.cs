using System;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.FileAccess;
using TECS.FileAccess.Mappers;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests.Chips;

public class AluTests
{
    private static DebugChip _alu = GetChip();

    private const string Zero = "0000000000000000";
    private const string Ten = "0000000000001010";
    private const string MinusTen = "1111111111110110";
    private const string Ones = "1111111111111111";
    private const string SomeValue = "1110111100111111";

    private const string True = "1";
    private const string False = "0";

    [Test]
    public void CanConstructAluDebugChip()
    {
        _alu.Should().NotBeNull();
        _alu.Inputs.Should().HaveCount(8);
        _alu.Inputs.Should().ContainKey(new("x"));
        _alu.Inputs.Should().ContainKey(new("y"));
        _alu.Inputs.Should().ContainKey(new("zx"));
        _alu.Inputs.Should().ContainKey(new("nx"));
        _alu.Inputs.Should().ContainKey(new("zy"));
        _alu.Inputs.Should().ContainKey(new("ny"));
        _alu.Inputs.Should().ContainKey(new("f"));
        _alu.Inputs.Should().ContainKey(new("no"));

        _alu.Outputs.Should().ContainKey(new("out"));
        _alu.Outputs.Should().ContainKey(new("zr"));
        _alu.Outputs.Should().ContainKey(new("ng"));

        _alu.Internals.Should().ContainKey(new("x0"));
        _alu.Internals.Should().ContainKey(new("y0"));
    }

    [Test]
    public void CanSetToZero()
    {
        SetValues(x: SomeValue, y: Ten, zx: true, nx: false, zy: true, ny: false, f: true, no: false);

        GetInternal("x0").Should().Be(Zero);
        GetInternal("y0").Should().Be(Zero);
        GetInternal("xy").Should().Be(Zero);
        GetOutput("out").Should().Be(Zero);
        GetOutput("zr").Should().Be(True);
        GetOutput("ng").Should().Be(False);
    }

    [Test]
    public void CanSetToOne()
    {
        SetValues(x: SomeValue, y: Ten, zx: true, nx: true, zy: true, ny: true, f: true, no: true);

        GetInternal("x0").Should().Be(Ones);
        GetInternal("y0").Should().Be(Ones);
        GetInternal("xy").Should().Be("1111111111111110");
        GetOutput("out").Should().Be("0000000000000001");
        GetOutput("zr").Should().Be(False);
        GetOutput("ng").Should().Be(False);
    }

    [Test]
    public void CanSetXToValue()
    {
        SetValues(x: SomeValue, y: Zero, f: true);

        GetInternal("x0").Should().Be(SomeValue);
        GetInternal("xy").Should().Be(SomeValue);
        GetOutput("out").Should().Be(SomeValue);
    }

    [Test]
    public void CanSetYToValue()
    {
        var alu = GetChip();

        SetValues(x: Zero, y: SomeValue, f: true);

        GetInternal("y0").Should().Be(SomeValue);
        GetInternal("xy").Should().Be(SomeValue);
        GetOutput("out").Should().Be(SomeValue);
    }

    private void SetValues(string x = "0000000000000000", string y = "0000000000000000", bool zx = false,
        bool nx = false, bool zy = false, bool ny = false, bool f = false, bool no = false)
    {
        _alu.Inputs[new("x")].SetValue(ConvertString(x));
        _alu.Inputs[new("y")].SetValue(ConvertString(y));
        _alu.Inputs[new("zx")].SetValue(zx ? BitValue.True : BitValue.False);
        _alu.Inputs[new("nx")].SetValue(nx ? BitValue.True : BitValue.False);
        _alu.Inputs[new("zy")].SetValue(zy ? BitValue.True : BitValue.False);
        _alu.Inputs[new("ny")].SetValue(ny ? BitValue.True : BitValue.False);
        _alu.Inputs[new("f")].SetValue(f ? BitValue.True : BitValue.False);
        _alu.Inputs[new("no")].SetValue(no ? BitValue.True : BitValue.False);
    }

    private static string GetInput(string name) =>
        ConvertBitValue(_alu.Inputs[new(name)].GetValue());

    private static string GetInternal(string name) =>
        ConvertBitValue(_alu.Internals[new(name)].GetValue());

    private static string GetOutput(string name) =>
        ConvertBitValue(_alu.Outputs[new(name)].GetValue());

    private static BitValue ConvertString(string input) => new(input.Select(c => c == '1').Reverse().ToArray());

    private static string ConvertBitValue(BitValue value) =>
        new(value.Value.Select(b => b ? '1' : '0').Reverse().ToArray());

    private static DebugChip GetChip()
    {
        var dataFolder = new DataFolder(Settings.DataFolder);

        var hdlFolder = dataFolder.HdlFolder;

        var chipData = hdlFolder.HdlFiles.Select(HdlToIntermediateMapper.Map).ToArray();

        var factory = new ChipBlueprintFactory(chipData);

        var debugFac = new DebugChipFactory(factory);

        var result = debugFac.Create(chipData.Single(cd => cd.Name.Value == "ALU"));

        if (result.Success)
        {
            _alu = result.Result ?? throw new InvalidOperationException("huh");
        }
        else
        {
            result.Errors.Should().BeEmpty();
            throw new InvalidOperationException("double huuuh");
        }

        return _alu;
    }
}