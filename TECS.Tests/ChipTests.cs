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
        
        var (pins, nands) = andChip.Outputs["out"].CountNodes(0);

        pins.Should().BeGreaterThan(2);
        nands.Should().Be(2);
    }

    [Test]
    public void CanBuildPrefusedAndChipBlueprintByHand()
    {
        var andChip = AndChipByHand(preFuse: true);

        var (pins, nands) = andChip.Outputs["out"].CountNodes(0);

        pins.Should().Be(2);
        nands.Should().Be(2);
    }

    [Test]
    public void CanFabricateAndChipFromBlueprint()
    {
        var bluePrint = AndChipByHand();

        var andChip = bluePrint.Fabricate();

        var (pins, nands) = andChip.Outputs["out"].CountNodes(2);

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
        
        andChip.Evaluate("out").Should().BeEquivalentTo(new[] { a && b });
    }

    private ChipBlueprint NandBlueprintByHand() => ChipBlueprintFactory.NandBlueprint();

    private ChipBlueprint NotBlueprintByHand()
    {
        // CHIP Not
        
        //     IN in;
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "in", new() }
        };
        
        //     OUT out;
        var outputPins = new Dictionary<string, NandPinNode>()
        {
            { "out", new() }
        };
        
        //     PARTS:
        //     Nand
        var nandChip = NandBlueprintByHand();
        // a=in
        nandChip.Inputs["a"].Parent = inputs["in"];
        // b=in
        nandChip.Inputs["b"].Parent = inputs["in"];

        //out=out
        outputPins["out"].Parent = nandChip.Outputs["out"];
        
        var outputs = outputPins.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value as INandTreeNode);

        return new ChipBlueprint("Not", inputs, outputs, false);
    }

    private ChipBlueprint AndChipByHand(bool preFuse = false)
    {
        // CHIP And
        
        //     IN a, b;
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", new() },
            { "b", new() }
        };
        
        //     OUT out;
        var outputPins = new Dictionary<string, NandPinNode>()
        {
            { "out", new() }
        };

        var internalPins = new Dictionary<string, NandPinNode>();
        
        //     PARTS:
        //     Not
        var notChip = NotBlueprintByHand();
        
        // in=mid
        internalPins.Add("mid", new()); // first mention of mid, make a mid pin
        
        notChip.Inputs["in"].Parent = internalPins["mid"];
        
        // out=out
        outputPins["out"].Parent = notChip.Outputs["out"];
        
        //     Nand
        var nandChip = NandBlueprintByHand();
        
        // a=a
        nandChip.Inputs["a"].Parent = inputs["a"];
        
        // b=b
        nandChip.Inputs["b"].Parent = inputs["b"];
        
        // out=mid
        internalPins["mid"].Parent = nandChip.Outputs["out"];

        var outputs = outputPins.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value as INandTreeNode);
        
        return new ChipBlueprint("And", inputs, outputs, preFuse);
    }
}