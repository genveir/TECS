using System;
using System.IO;
using System.Linq;
using TECS.HDLSimulator.Chips;

namespace TECS.HDLSimulator;

public class Simulator
{
    public Simulator(string dataFolder)
    {
        var folder = new HdlFolder(Path.Combine(dataFolder, "HDL"));

        var files = folder.GetFiles();

        var contents = files.Select(f => f.GetContents());

        var descriptions = contents.Select(HdlParser.ParseDescription).ToArray();

        var bluePrintFactory = new ChipBlueprintFactory(descriptions);

        var blueprint = bluePrintFactory.BuildBlueprint(descriptions.Single(desc => desc.Name == "And"));

        var chip = blueprint.Fabricate();

        chip.Inputs["a"].Value = new[] { true };
        chip.Inputs["b"].Value = new[] { false };
        
        Console.WriteLine(chip.Evaluate("out").Single());
    }
}