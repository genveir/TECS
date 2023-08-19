using FluentAssertions;
using NUnit.Framework;
using TECS.HDLSimulator.Chips;
using TECS.Tests.Builders;

namespace TECS.Tests;

public class NandTests
{
    private static bool CalcNand(bool a, bool b) => !(a && b);
    
    [Test]
    public void CanCreateNand()
    {
        var pinA = new PinBuilder(1).Build();
        var pinB = new PinBuilder(1).Build();
        var pinOut = new PinBuilder(1).Build();
        
        new NandBuilder(pinA, pinB, pinOut).Build();
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void NandStartsWithSetOutput(bool a, bool b)
    {
        var pinA = new PinBuilder(1).WithValue(a).Build();
        var pinB = new PinBuilder(1).WithValue(b).Build();
        var pinOut = new PinBuilder(1).Build();

        new NandBuilder(pinA, pinB, pinOut).Build();

        pinOut.Value.Should().BeEquivalentTo(new[] { CalcNand(a, b) });
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void NandUpdatesToCorrectValues(bool a, bool b)
    {
        var pinA = new PinBuilder(1).Build();
        var pinB = new PinBuilder(1).Build();
        var pinOut = new PinBuilder(1).Build();

        new NandBuilder(pinA, pinB, pinOut).Build();

        pinA.Value = new[] { a };
        pinB.Value = new[] { b };

        pinOut.Value.Should().BeEquivalentTo(new[] { CalcNand(a, b) });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CanConnectBothInputsToOnePin(bool input)
    {
        var pinA = new PinBuilder(1).WithValue(input).Build();
        var pinOut = new PinBuilder(1).Build();

        new NandBuilder(pinA, pinA, pinOut).Build();

        pinOut.Value.Should().BeEquivalentTo(new[] { CalcNand(input, input) });
    }

    [TestCase(true)]
    [TestCase(false)]
    public void CanChainNands(bool input)
    {
        var pinA = new PinBuilder(1).WithValue(input).Build();
        var pinMid = new PinBuilder(1).Build();
        var pinOut = new PinBuilder(1).Build();

        new NandBuilder(pinA, pinA, pinMid).Build();
        new NandBuilder(pinMid, pinMid, pinOut).Build();

        var step1out = CalcNand(input, input);
        var output = CalcNand(step1out, step1out);
        
        pinOut.Value.Should().BeEquivalentTo(new[] { output });
    }

    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    public void CanStaggerNands(bool a, bool b, bool c)
    {
        var pinA = new PinBuilder(1).WithValue(a).Build();
        var pinB = new PinBuilder(1).WithValue(b).Build();
        var pinC = new PinBuilder(1).WithValue(c).Build();
        var pinMid = new PinBuilder(1).Build();
        var pinOut = new PinBuilder(1).Build();

        new NandBuilder(pinB, pinC, pinMid).Build();
        new NandBuilder(pinA, pinMid, pinOut).Build();

        var output = CalcNand(a, CalcNand(b, c));

        pinOut.Value.Should().BeEquivalentTo(new[] { output });
    }

    [TestCase(false, false, false)]
    [TestCase(false, false, true)]
    [TestCase(false, true, false)]
    [TestCase(false, true, true)]
    [TestCase(true, false, false)]
    [TestCase(true, false, true)]
    [TestCase(true, true, false)]
    [TestCase(true, true, true)]
    public void CanConnectNandsToSamePin(bool a, bool b, bool c)
    {
        var pinA = new PinBuilder(1).WithValue(a).Build();
        var pinB = new PinBuilder(1).WithValue(b).Build();
        var pinC = new PinBuilder(1).WithValue(c).Build();

        var pinOut1 = new PinBuilder(1).Build();
        var pinOut2 = new PinBuilder(1).Build();

        new NandBuilder(pinA, pinB, pinOut1).Build();
        new NandBuilder(pinB, pinC, pinOut2).Build();

        pinOut1.Value.Should().BeEquivalentTo(new[] { CalcNand(a, b) });
        pinOut2.Value.Should().BeEquivalentTo(new[] { CalcNand(b, c) });
    }
}