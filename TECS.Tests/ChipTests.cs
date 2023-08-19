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

    private Chip NandChipByHand()
    {
        var nandNode = new NandNode();

        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", nandNode.A },
            { "b", nandNode.B }
        };

        var outputName = "out";

        return new Chip(inputs, outputName, nandNode);
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
        var outputName = "out";
        
        //     PARTS:
        //     Nand
        var nandChip = NandChipByHand();
        // a=in
        nandChip.Inputs["a"].Parent = inPin;
        // b=in
        nandChip.Inputs["b"].Parent = inPin;

        //out=out
        outPin.Parent = nandChip.Output;

        return new Chip(inputs, outputName, outPin);
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
        var outputName = "out";
        
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

        return new Chip(inputs, outputName, outPin);
    }
}