using System;
using System.IO;
using System.Linq;
using TECS.FileAccess;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.HDL;

namespace TECS.Tests;

public static class TestDataFactory
{
    public static object?[][] Create(string dataFolder)
    {
        var hdlFolder = new DataFolder(dataFolder).HdlFolder;

        var hdlFiles = hdlFolder.HdlFiles;

        var hdlData = hdlFiles.Select(hf => hf.GetContents());

        var parsedHdlData = hdlData.Select(HdlParser.ParseDescription);

        var blueprintFactory = new ChipBlueprintFactory(parsedHdlData);

        var hdlPath = Path.Join(dataFolder, "HDL");

        var testfiles = hdlFolder.TestFiles;

        var data = new object?[testfiles.Length][];
        for (int n = 0; n < testfiles.Length; n++)
        {
            var testfile = testfiles[n];

            var testFileContent = testfile.GetContents();

            var testName = testfile.Name;
            
            var typename = testFileContent.Single(l => l.StartsWith("load"))
                .Replace("load ", "")
                .Replace(".hdl,", "")
                .Trim();

            var comparisonFile = hdlFolder.ComparisonFiles.Single(cmp => cmp.Name == testName);
            var comparisonFileContent = comparisonFile.GetContents();

            var desc = blueprintFactory.GetChipDescription(typename);
            
            if (desc == null) throw new InvalidOperationException($"description {typename} does not exist");
            var blueprint = blueprintFactory.BuildBlueprint(desc);

            Chip? chip = null;
            if (!blueprint.ValidationErrors.Any())
                chip = blueprint.CopyToBlueprintInstance().Fabricate();

            var newChipTestData = new object?[]
            {
                testName,
                blueprint.ValidationErrors,
                chip,
                testfile,
                comparisonFile
            };

            data[n] = newChipTestData;
        }

        return data;
    }
}