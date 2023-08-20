using FluentAssertions;
using NUnit.Framework;
using TECS.HDLSimulator.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.Tests;

public class ChipTests
{
    [Test]
    public void CanBuildAndChipBlueprintByHand()
    {
        var andChip = AndChipByHand();
        
        var (pins, nands) = andChip.Output.CountNodes(0);

        pins.Should().BeGreaterThan(2);
        nands.Should().Be(2);
    }

    [Test]
    public void CanBuildPrefusedAndChipBlueprintByHand()
    {
        var andChip = AndChipByHand(preFuse: true);

        var (pins, nands) = andChip.Output.CountNodes(0);

        pins.Should().Be(2);
        nands.Should().Be(2);
    }

    [Test]
    public void CanFabricateAndChipFromBlueprint()
    {
        var bluePrint = AndChipByHand();

        var andChip = bluePrint.Fabricate();

        var (pins, nands) = andChip.Output.CountNodes(2);

        pins.Should().Be(2);
        nands.Should().Be(2);
    }

    [TestCase(true, true)]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase(false, false)]
    public void AndChipFunctions(bool a, bool b)
    {
        var bluePrint = AndChipByHand();

        var andChip = bluePrint.Fabricate();
        
        andChip.Inputs["a"].Value = new[] { a };
        andChip.Inputs["b"].Value = new[] { b };
        
        andChip.Evaluate().Should().BeEquivalentTo(new[] { a && b });
    }

    private ChipBlueprint NandBlueprintByHand()
    {
        var nandNode = new NandNode();

        nandNode.TryGetInputPins(out var inputNodes);
        
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", inputNodes.a },
            { "b", inputNodes.b }
        };

        return new ChipBlueprint("Nand", inputs, "out", nandNode, false);
    }

    private ChipBlueprint NotBlueprintByHand()
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
        var nandChip = NandBlueprintByHand();
        // a=in
        nandChip.Inputs["a"].Parent = inPin;
        // b=in
        nandChip.Inputs["b"].Parent = inPin;

        //out=out
        outPin.Parent = nandChip.Output;

        return new ChipBlueprint("Not", inputs, outputName, outPin, false);
    }

    private ChipBlueprint AndChipByHand(bool preFuse = false)
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
        var notChip = NotBlueprintByHand();
        
        // in=mid
        var midPin = new NandPinNode(); // first mention of mid, make a mid pin
        notChip.Inputs["in"].Parent = midPin;
        
        // out=out
        outPin.Parent = notChip.Output;
        
        //     Nand
        var nandChip = NandBlueprintByHand();
        
        // a=a
        nandChip.Inputs["a"].Parent = aPin;
        
        // b=b
        nandChip.Inputs["b"].Parent = bPin;
        
        // out=mid
        midPin.Parent = nandChip.Output;

        return new ChipBlueprint("And", inputs, outputName, outPin, preFuse);
    }
}