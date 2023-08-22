using System;
using System.IO;
using System.Linq;
using TECS.HDLSimulator.Chips;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests;

public static class TestDataFactory
{
    public static object?[][] Create(string dataFolder)
    {
        var blueprintFactory = ChipBlueprintFactory.FromFilesystem(dataFolder);

        var hdlPath = Path.Join(dataFolder, "HDL");

        var testfiles = Directory.GetFiles(hdlPath, "*.tst");

        var data = new object?[testfiles.Length][];
        for (int n = 0; n < testfiles.Length; n++)
        {
            var testfile = testfiles[n];

            var typename = Path.GetFileNameWithoutExtension(testfile);

            var testFileContent = File.ReadAllLines(testfile);

            var comparisonFileName = testfile.Replace(".tst", ".cmp");
            var comparisonFileContent = File.ReadAllLines(comparisonFileName);

            var desc = blueprintFactory.GetChipDescription(typename);
            
            if (desc == null) throw new InvalidOperationException("description does not exist");
            var blueprint = blueprintFactory.BuildBlueprint(desc);

            Chip? chip = null;
            if (!blueprint.ValidationErrors.Any())
                chip = blueprint.CopyToBlueprintInstance().Fabricate();

            var newChipTestData = new object?[]
            {
                typename,
                blueprint.ValidationErrors,
                chip,
                new TestFile(testFileContent),
                new ComparisonFile(comparisonFileContent)
            };

            data[n] = newChipTestData;
        }

        return data;
    }
}