using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;

namespace TECS.Tests.Chips;

public class LatchTests : ChipTestFramework
{
    public LatchTests() : base("Latch") { }

    // ReSharper disable InconsistentNaming
    private static class Inputs
    {
        public static readonly NamedNodeGroupName S = new("s");
        public static readonly NamedNodeGroupName R = new("r");
    }

    private static class Outputs
    {
        public static readonly NamedNodeGroupName OUT = new("out");
    }

    private static class Internals
    {
        public static readonly NamedNodeGroupName N0 = new("n0");
        public static readonly NamedNodeGroupName N1 = new("n1");
    }

    private const string FALSE = "0";
    private const string TRUE = "1";
    // ReSharper enable InconsistentNaming
    
    [Test]
    public void CanSetLatchToTrue()
    {
        TestChip.SetInput(Inputs.S, BitValue.False);
        TestChip.SetInput(Inputs.R, BitValue.True);

        Evaluate();

        GetInternal(Internals.N0).Should().Be(TRUE);
        GetInternal(Internals.N1).Should().Be(FALSE);
        GetOutput(Outputs.OUT).Should().Be(TRUE);
    }

    [Test]
    public void CanSetLatchToFalse()
    {
        TestChip.SetInput(Inputs.S, BitValue.True);
        TestChip.SetInput(Inputs.R, BitValue.False);

        Evaluate();

        GetInternal(Internals.N0).Should().Be(FALSE);
        GetInternal(Internals.N1).Should().Be(TRUE);
        GetOutput(Outputs.OUT).Should().Be(FALSE);
    }
    
    [TestCase(true)]
    [TestCase(false)]
    public void NoChangeWhenBothInputsTrue(bool previousState)
    {
        if (previousState) CanSetLatchToTrue();
        else CanSetLatchToFalse();
        
        TestChip.SetInput(Inputs.S, BitValue.True);
        TestChip.SetInput(Inputs.R, BitValue.True);

        Evaluate();

        GetOutput(Outputs.OUT).Should().Be(previousState ? TRUE : FALSE);
    }

    [TestCase(true)]
    [TestCase(false)]
    public void LatchReturnsSomethingOnInvalidState(bool previousState)
    {
        if (previousState) CanSetLatchToTrue();
        else CanSetLatchToFalse();
        
        TestChip.SetInput(Inputs.S, BitValue.False);
        TestChip.SetInput(Inputs.R, BitValue.False);
        
        Evaluate();

        GetOutput(Outputs.OUT).Should().BeOneOf(TRUE, FALSE);
    }

    [Test]
    public void ProvidedCase()
    {
        RefreshChip();
        
        TestChip.SetInput(Inputs.S, BitValue.False);
        TestChip.SetInput(Inputs.R, BitValue.True);
        
        Evaluate();

        GetOutput(Outputs.OUT).Should().Be(TRUE);
        
        TestChip.SetInput(Inputs.S, BitValue.True);
        TestChip.SetInput(Inputs.R, BitValue.True);
        
        Evaluate();
        
        GetOutput(Outputs.OUT).Should().Be(TRUE);
        
        TestChip.SetInput(Inputs.S, BitValue.True);
        TestChip.SetInput(Inputs.R, BitValue.False);
        
        Evaluate();

        GetOutput(Outputs.OUT).Should().Be(FALSE);
        
        TestChip.SetInput(Inputs.S, BitValue.True);
        TestChip.SetInput(Inputs.R, BitValue.True);
        
        Evaluate();
        
        GetOutput(Outputs.OUT).Should().Be(FALSE);
    }
}