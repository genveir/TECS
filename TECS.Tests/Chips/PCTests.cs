using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.Tests.Chips;

public class PcTests : ChipTestFramework
{
    public PcTests() : base("PC") { }
    
    // ReSharper disable InconsistentNaming
    private static class Inputs
    {
        public static readonly NamedNodeGroupName IN = new("in");
        public static readonly NamedNodeGroupName LOAD = new("load");
        public static readonly NamedNodeGroupName INC = new("inc");
        public static readonly NamedNodeGroupName RESET = new("reset");
    }

    private static class Outputs
    {
        public static readonly NamedNodeGroupName OUT = new("out");
    }

    private static class Internals
    {
        public static readonly NamedNodeGroupName M0 = new("m0");
        public static readonly NamedNodeGroupName MINC = new("minc");
        public static readonly NamedNodeGroupName MLOAD = new("mload");
        public static readonly NamedNodeGroupName MRESET = new("mreset");
        public static readonly NamedNodeGroupName MOUT = new("mout");
    }

    private const string ZERO = "0000000000000000";
    private const string ONE = "0000000000000001";
    private const string TWO = "0000000000000010";
    private const string THREE = "0000000000000011";
    // ReSharper enable InconsistentNaming
    
    [SetUp]
    public void Setup()
    {
        RefreshChip();
    }

    [TestCase(0)]
    [TestCase(1)]
    [TestCase(-1)]
    public void CanLeaveAtZero(int inValue)
    {
        SetInputs(inValue, load: false, inc: false, reset: false);

        for (int n = 0; n < 10; n++)
        {
            var result = EvalAll();

            result.M0.Should().Be(ONE);
            result.MINC.Should().Be(ZERO);
            result.MLOAD.Should().Be(ZERO);
            result.MRESET.Should().Be(ZERO);
            result.OUT.Should().Be(ZERO);
        }
    }
    
    [Test]
    public void CanSetToValue()
    {
        for (int n = -10; n < 10; n += 2)
        {
            var inAsBinaryString = To16BitString(n);
            var plusOne = To16BitString(n + 1);
        
            SetInputs(n, load: true, inc: false, reset: false);
            
            var result = EvalAll();

            result.M0.Should().Be(plusOne);
            result.MINC.Should().Be(inAsBinaryString);
            result.MLOAD.Should().Be(inAsBinaryString);
            result.MRESET.Should().Be(inAsBinaryString);
            result.OUT.Should().Be(inAsBinaryString);
        }
    }

    [Test]
    public void ValueDoesNotChangeUntilClockTocks()
    {
        SetInputs(2, load: true, inc: false, reset: false);
        
        Evaluate(IncrementMode.None);
        var result = ToDiagnostics();
        
        result.M0.Should().Be(ONE);
        result.MINC.Should().Be(ZERO);
        result.MLOAD.Should().Be(TWO);
        result.MRESET.Should().Be(TWO);
        result.OUT.Should().Be(ZERO);
        
        Evaluate(IncrementMode.Single);
        result = ToDiagnostics();
        
        result.M0.Should().Be(ONE);
        result.MINC.Should().Be(ZERO);
        result.MLOAD.Should().Be(TWO);
        result.MRESET.Should().Be(TWO);
        result.OUT.Should().Be(ZERO);
        
        Evaluate(IncrementMode.Single);
        result = ToDiagnostics();
        
        result.M0.Should().Be(THREE);
        result.MINC.Should().Be(TWO);
        result.MLOAD.Should().Be(TWO);
        result.MRESET.Should().Be(TWO);
        result.OUT.Should().Be(TWO);
    }

    [Test]
    public void CanIncrement()
    {
        SetInputs(0, load: false, inc: true, reset: false);
        
        for (int n = 1; n < 10; n++)
        {
            var result = EvalAll();

            var current = To16BitString(n);
            var next = To16BitString(n + 1);

            result.M0.Should().Be(next);
            result.MINC.Should().Be(next);
            result.MLOAD.Should().Be(next);
            result.MRESET.Should().Be(next);
            result.OUT.Should().Be(current);
        }
    }

    private string To16BitString(int shortValue) => new BitValue((short)shortValue, 16).AsBinaryString();
    
    private void SetInputs(int @in, bool load, bool inc, bool reset)
    {
        TestChip.SetInput(Inputs.IN, new((short)@in, 16));
        TestChip.SetInput(Inputs.LOAD, new(load));
        TestChip.SetInput(Inputs.INC, new(inc));
        TestChip.SetInput(Inputs.RESET, new(reset));
    }

    private PCDiagnostics EvalAll()
    {
        Evaluate(IncrementMode.Single);
        GetOutput(Outputs.OUT);
        Evaluate(IncrementMode.Single);

        GetInternal(Internals.MOUT).Should().Be(GetOutput(Outputs.OUT));

        return ToDiagnostics();
    }

    private PCDiagnostics ToDiagnostics() =>
        new(
            m0: GetInternal(Internals.M0),
            minc: GetInternal(Internals.MINC),
            mload: GetInternal(Internals.MLOAD),
            mreset: GetInternal(Internals.MRESET),
            @out: GetOutput(Outputs.OUT)
        );

    private class PCDiagnostics
    {
        public string M0;
        public string MINC;
        public string MLOAD;
        public string MRESET;
        public string OUT;

        public PCDiagnostics(string m0, string minc, string mload, string mreset, string @out)
        {
            M0 = m0;
            MINC = minc;
            MLOAD = mload;
            MRESET = mreset;
            OUT = @out;
        }
    }
}