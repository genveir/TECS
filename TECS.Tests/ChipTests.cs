using FluentAssertions;
using NUnit.Framework;
using TECS.HDLSimulator.Chips;

namespace TECS.Tests;

public class ChipTests
{
    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void CanBuildAndChipByHand(bool a, bool b)
    {
        var andChip = AndChipByHand();

        andChip.Inputs["a"].Value = new[] { a };
        andChip.Inputs["b"].Value = new[] { b };

        andChip.Evaluate().Should().BeEquivalentTo(new[] { a && b });
    }

    [Test]
    public void AndChipIsNotFusedByDefault()
    {
        var andChip = AndChipByHand();

        var (pins, nands) = andChip.Output.CountNodes(0);

        pins.Should().BeGreaterThan(2);
        nands.Should().Be(2);
    }
    
    [Test]
    public void CanFuseAndChip()
    {
        var andChip = AndChipByHand();

        andChip.Fuse();

        var (pins, nands) = andChip.Output.CountNodes(1);

        pins.Should().Be(2);
        nands.Should().Be(2);
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void FusedAndChipFunctions(bool a, bool b)
    {
        var andChip = AndChipByHand();

        andChip.Fuse();
        
        andChip.Inputs["a"].Value = new[] { a };
        andChip.Inputs["b"].Value = new[] { b };
        
        andChip.Evaluate().Should().BeEquivalentTo(new[] { a && b });
    }

    private Chip NandChipByHand()
    {
        var nandNode = new NandNode();

        nandNode.TryGetInputPins(out var inputNodes);
        
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", inputNodes.a },
            { "b", inputNodes.b }
        };

        return new Chip(inputs, nandNode);
    }

    private Chip NotChipByHand()
    {
        // CHIP Not
        
        //     IN in;
        var inPin = new NandPinNode();
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "in", inPin }
        };
        
        //     OUT out;
        var outPin = new NandPinNode();
        
        //     PARTS:
        //     Nand
        var nandChip = NandChipByHand();
        // a=in
        nandChip.Inputs["a"].Parent = inPin;
        // b=in
        nandChip.Inputs["b"].Parent = inPin;

        //out=out
        outPin.Parent = nandChip.Output;

        return new Chip(inputs, outPin);
    }

    private Chip AndChipByHand()
    {
        // CHIP And
        
        //     IN a, b;
        var aPin = new NandPinNode();
        var bPin = new NandPinNode();
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", aPin },
            { "b", bPin }
        };
        
        //     OUT out;
        var outPin = new NandPinNode();
        
        //     PARTS:
        //     Not
        var notChip = NotChipByHand();
        
        // in=mid
        var midPin = new NandPinNode(); // first mention of mid, make a mid pin
        notChip.Inputs["in"].Parent = midPin;
        
        // out=out
        outPin.Parent = notChip.Output;
        
        //     Nand
        var nandChip = NandChipByHand();
        
        // a=a
        nandChip.Inputs["a"].Parent = aPin;
        
        // b=b
        nandChip.Inputs["b"].Parent = bPin;
        
        // out=mid
        midPin.Parent = nandChip.Output;

        return new Chip(inputs, outPin);
    }
}