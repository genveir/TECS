using System;
using System.Linq;
using TECS.DataIntermediates.Chip.Mappers;
using TECS.FileAccess;
using TECS.FileAccess.Mappers;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;

namespace TECS.Tests;

public static class TestDataFactory
{
    public static object?[][] Create(string dataFolder)
    {
        var hdlFolder = new DataFolder(dataFolder).HdlFolder;

        var hdlFiles = hdlFolder.HdlFiles;
        
        var intermediates = 
            hdlFiles.Select(HdlToIntermediateMapper.Map);
        
        var parsedHdlData = 
            intermediates.Select(IntermediateToChipDescriptionMapper.Map);

        var blueprintFactory = new ChipBlueprintFactory(parsedHdlData);

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