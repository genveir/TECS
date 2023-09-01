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
    private const string Ones = "1111111111111111";
    private const string SomeValue = "1110111100111111";

    private const string True = "1";
    private const string False = "0";

    // ReSharper disable InconsistentNaming
    private static class Inputs
    {
        public static readonly NamedNodeGroupName X = new("x");
        public static readonly NamedNodeGroupName Y = new("y");
        public static readonly NamedNodeGroupName ZX = new("zx");
        public static readonly NamedNodeGroupName NX = new("nx");
        public static readonly NamedNodeGroupName ZY = new("zy");
        public static readonly NamedNodeGroupName NY = new("ny");
        public static readonly NamedNodeGroupName F = new("f");
        public static readonly NamedNodeGroupName NO = new("no");

        public static readonly NamedNodeGroupName[] All = new[] { X, Y, ZX, NX, ZY, NY, F, NO };
    }

    private static class Outputs
    {
        public static readonly NamedNodeGroupName OUT = new("out");
        public static readonly NamedNodeGroupName NG = new("ng");
        public static readonly NamedNodeGroupName ZR = new("zr");

        public static readonly NamedNodeGroupName[] All = new[] { OUT, NG, ZR };
    }

    // not exhaustive, but makes sure we can test that the internals we use in tests
    // are still in the chip implementation
    private static class Internals
    {
        public static readonly NamedNodeGroupName X0 = new("x0");
        public static readonly NamedNodeGroupName Y0 = new("y0");
        public static readonly NamedNodeGroupName XY = new("xy");

        public static readonly NamedNodeGroupName[] AllSpecified = new[] { X0, Y0, XY };
    }
    // ReSharper enable InconsistentNaming
    
    [Test]
    public void CanConstructAluDebugChip()
    {
        _alu.Should().NotBeNull();
        _alu.InputNames.Should().HaveCount(8);
        _alu.InputNames.Should().Contain(Inputs.All);

        _alu.OutputNames.Should().HaveCount(3);
        _alu.OutputNames.Should().Contain(Outputs.All);

        _alu.InternalNames.Should().Contain(Internals.AllSpecified);
    }

    [Test]
    public void CanSetToZero()
    {
        SetValues(x: SomeValue, y: Ten, zx: true, nx: false, zy: true, ny: false, f: true, no: false);

        GetInternal(Internals.X0).Should().Be(Zero);
        GetInternal(Internals.Y0).Should().Be(Zero);
        GetInternal(Internals.XY).Should().Be(Zero);
        GetOutput(Outputs.OUT).Should().Be(Zero);
        GetOutput(Outputs.ZR).Should().Be(True);
        GetOutput(Outputs.NG).Should().Be(False);
    }

    [Test]
    public void CanSetToOne()
    {
        SetValues(x: SomeValue, y: Ten, zx: true, nx: true, zy: true, ny: true, f: true, no: true);

        GetInternal(Internals.X0).Should().Be(Ones);
        GetInternal(Internals.Y0).Should().Be(Ones);
        GetInternal(Internals.XY).Should().Be("1111111111111110");
        GetOutput(Outputs.OUT).Should().Be("0000000000000001");
        GetOutput(Outputs.ZR).Should().Be(False);
        GetOutput(Outputs.NG).Should().Be(False);
    }

    [Test]
    public void CanSetXToValue()
    {
        SetValues(x: SomeValue, y: Zero, f: true);

        GetInternal(Internals.X0).Should().Be(SomeValue);
        GetInternal(Internals.XY).Should().Be(SomeValue);
        GetOutput(Outputs.OUT).Should().Be(SomeValue);
    }

    [Test]
    public void CanSetYToValue()
    {
        SetValues(x: Zero, y: SomeValue, f: true);

        GetInternal(Internals.Y0).Should().Be(SomeValue);
        GetInternal(Internals.XY).Should().Be(SomeValue);
        GetOutput(Outputs.OUT).Should().Be(SomeValue);
    }

    private void SetValues(string x = "0000000000000000", string y = "0000000000000000", bool zx = false,
        bool nx = false, bool zy = false, bool ny = false, bool f = false, bool no = false)
    {
        _alu.SetInput(Inputs.X, ConvertString(x));
        _alu.SetInput(Inputs.Y, ConvertString(y));
        _alu.SetInput(Inputs.ZX, zx ? BitValue.True : BitValue.False);
        _alu.SetInput(Inputs.NX, nx ? BitValue.True : BitValue.False);
        _alu.SetInput(Inputs.ZY, zy ? BitValue.True : BitValue.False);
        _alu.SetInput(Inputs.NY, ny ? BitValue.True : BitValue.False);
        _alu.SetInput(Inputs.F, f ? BitValue.True : BitValue.False);
        _alu.SetInput(Inputs.NO, no ? BitValue.True : BitValue.False);
    }

    private static string GetInternal(NamedNodeGroupName name) =>
        ConvertBitValue(_alu.GetInternal(name));

    private static string GetOutput(NamedNodeGroupName name) =>
        ConvertBitValue(_alu.GetOutput(name));

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