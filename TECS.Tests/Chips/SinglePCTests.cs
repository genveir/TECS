using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.Tests.Chips;

public class SinglePcTests : ChipTestFramework
{
    public SinglePcTests() : base("PCSingle") { }
    
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
    // ReSharper enable InconsistentNaming

    [SetUp]
    public void Setup()
    {
        RefreshChip();
    }

    [TestCase(false)]
    [TestCase(true)]
    public void CanLeaveAtZero(bool inValue)
    {
        SetInputs(inValue, load: false, inc: false, reset: false);

        for (int n = 0; n < 10; n++)
        {
            var result = EvalAll();

            result.MRES.Should().Be(inValue ? TRUE : FALSE);
            result.MRI.Should().Be(inValue ? TRUE : FALSE);
            result.MINC.Should().Be(TRUE);
            result.MLOADRI.Should().Be(FALSE);
            result.MLOAD.Should().Be(FALSE);
            result.OUT.Should().Be(FALSE);
        }
    }
    
    [Test]
    public void CanSetToValue()
    {
        SetInputs(true, load: true, inc: false, reset: false);
        
        for (int n = 0; n < 10; n++)
        {
            var result = EvalAll();

            result.MRES.Should().Be(TRUE);
            result.MRI.Should().Be(TRUE);
            result.MINC.Should().Be(n == 0 ? TRUE : FALSE);
            result.MLOADRI.Should().Be(FALSE);
            result.MLOAD.Should().Be(TRUE);
            result.OUT.Should().Be(n == 0 ? FALSE : TRUE);
        }
    }

    [Test]
    public void CanIncrement()
    {
        SetInputs(false, load: false, inc: true, reset: false);

        for (int n = 0; n < 10; n++)
        {
            var result = EvalAll();
            
            result.MRES.Should().Be(FALSE);
            result.MRI.Should().Be(TRUE);
            result.MINC.Should().Be(TRUE);
            result.MLOADRI.Should().Be(TRUE);
            result.MLOAD.Should().Be(TRUE);
            result.OUT.Should().Be(FALSE);
        }
    }
    
    private void SetInputs(bool @in, bool load, bool inc, bool reset)
    {
        TestChip.SetInput(Inputs.IN, new(@in));
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