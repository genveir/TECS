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
        public static readonly NamedNodeGroupName MRES = new("mres");
        public static readonly NamedNodeGroupName MRI = new("mri");
        public static readonly NamedNodeGroupName MINC = new("minc");
        public static readonly NamedNodeGroupName MLOADRI = new("mloadri");
        public static readonly NamedNodeGroupName MLOAD = new("mload");
        public static readonly NamedNodeGroupName MOUT = new("mout");
    }

    private const string FALSE = "0";
    private const string TRUE = "1";

    private const string ZERO = "0000000000000000";
    private const string ONE = "0000000000000001";
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
        var inAsBinaryString = To16BitString(inValue);
        
        SetInputs(inValue, load: false, inc: false, reset: false);

        for (int n = 0; n < 10; n++)
        {
            var result = EvalAll();

            result.MRES.Should().Be(inAsBinaryString);
            result.MRI.Should().Be(inAsBinaryString);
            result.MINC.Should().Be(ONE);
            result.MLOADRI.Should().Be(FALSE);
            result.MLOAD.Should().Be(FALSE);
            result.OUT.Should().Be(ZERO);
        }
    }

    [Test]
    public void CanIncrement()
    {
        SetInputs(0, load: false, inc: true, reset: false);

        for (int n = 0; n < 10; n++)
        {
            var result = EvalAll();

            var current = To16BitString(n);
            var next = To16BitString(n + 1);
            
            result.MRES.Should().Be(ZERO);
            result.MRI.Should().Be(next);
            result.MINC.Should().Be(next);
            result.MLOADRI.Should().Be(TRUE);
            result.MLOAD.Should().Be(TRUE);
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
        var midwayOut = GetOutput(Outputs.OUT);
        Evaluate(IncrementMode.Single);

        GetInternal(Internals.MOUT).Should().Be(GetOutput(Outputs.OUT));
        
        return new(
            mres: GetInternal(Internals.MRES),
            mri: GetInternal(Internals.MRI),
            minc: GetInternal(Internals.MINC),
            mloadri: GetInternal(Internals.MLOADRI),
            mload: GetInternal(Internals.MLOAD),
            @out: GetOutput(Outputs.OUT)
        );
    }

    private class PCDiagnostics
    {
        public string MRES;
        public string MRI;
        public string MINC;
        public string MLOADRI;
        public string MLOAD;
        public string OUT;

        public PCDiagnostics(string mres, string mri, string minc, string mloadri, string mload, string @out)
        {
            MRES = mres;
            MRI = mri;
            MINC = minc;
            MLOADRI = mloadri;
            MLOAD = mload;
            OUT = @out;
        }
    }
}