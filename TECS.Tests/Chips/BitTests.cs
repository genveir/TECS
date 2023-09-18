using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Values;
using TECS.HDLSimulator.Chips.Chips;

namespace TECS.Tests.Chips;

public class BitTests : ChipTestFramework
{
    public BitTests() : base("Bit") { }

    // ReSharper disable InconsistentNaming
    private static class Inputs
    {
        public static readonly NamedNodeGroupName IN = new("in");
        public static readonly NamedNodeGroupName LOAD = new("load");
    }

    private static class Outputs
    {
        public static readonly NamedNodeGroupName OUT = new("out");
    }

    private static class Internals
    {
        public static readonly NamedNodeGroupName M0 = new("m0");
        public static readonly NamedNodeGroupName D = new("d");
    }

    private const string TRUE = "1";
    private const string FALSE = "0";
    // ReSharper enable InconsistentNaming
    
    [SetUp]
    public void Setup()
    {
        Clock.Instance.Reset();
        RefreshChip();
        
        TestChip.SetInput(Inputs.IN, new BitValue(true));
        TestChip.SetInput(Inputs.LOAD, new BitValue(true));
    }

    [Test]
    public void BitValueDoesNotChangeOnTick()
    {
        Evaluate(IncrementMode.Single);

        GetInternal(Internals.M0).Should().Be(TRUE);
        GetInternal(Internals.D).Should().Be(FALSE);
        GetOutput(Outputs.OUT).Should().Be(FALSE);
    }

    [Test]
    public void BitValueDoesChangeOnTock()
    {
        Evaluate(IncrementMode.Double);

        GetInternal(Internals.M0).Should().Be(TRUE);
        GetInternal(Internals.D).Should().Be(TRUE);
        GetOutput(Outputs.OUT).Should().Be(TRUE);
    }
}