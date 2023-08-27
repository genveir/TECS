using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Chip.Mappers;
using TECS.DataIntermediates.Test;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;
using TECS.HDLSimulator.ChipDescriptions;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.Tests;


public static class TestDataFactory
{
    private static ChipBlueprintFactory? _factory;

    private static ChipBlueprintFactory GetFactory(string dataFolder)
    {
        if (_factory == null)
        {
            var hdlFolder = new DataFolder(dataFolder).HdlFolder;

            List<ChipDescription> parseableChipDescriptions = new();
            foreach (var hdlFile in hdlFolder.HdlFiles)
            {
                var chipData = GetChipData(hdlFile);
                if (chipData != null)
                {
                    var desc = IntermediateToChipDescriptionMapper.Map(chipData);
                    parseableChipDescriptions.Add(desc);
                }
            }

            _factory = new ChipBlueprintFactory(parseableChipDescriptions);
        }

        return _factory;
    }

    private static ChipData? GetChipData(HdlFile file)
    {
        try
        {
            return HdlToIntermediateMapper.Map(file);
        }
        catch (Exception e)
        {
            return null;
        }
    } 
    
    public static object?[][] Create(string dataFolder, string name)
    {
        var hdlFolder = new DataFolder(dataFolder).HdlFolder;

        var factory = GetFactory(dataFolder);
        
        var testFile = hdlFolder.TestFiles.SingleOrDefault(tst => tst.Name == name);
        if (testFile == null)
            return CreateFailedTestCreationResult($"could not find test file {name}");

        var testData = TstToIntermediateMapper.Map(hdlFolder, testFile);

        var description = factory.GetChipDescription(testData.ChipToTest.Name.Value);
        if (description == null)
            return CreateFailedTestCreationResult($"could not find description for {testData.ChipToTest.Name.Value} in factory, likely it was not successfully mapped");

        var storedBlueprint = factory.BuildBlueprint(description);
        if (storedBlueprint.ValidationErrors.Any())
            return CreateFailedTestCreationResult(storedBlueprint.ValidationErrors);

        var chip = storedBlueprint.CopyToBlueprintInstance().Fabricate();

        List<object?[]> result = new();
        for (int n = 0; n < testData.Tests.Length; n++)
        {
            result.Add(CreateSingleTest(new(), chip, testData, n));
        }
        
        return result.ToArray();
    }

    private static object?[][] CreateFailedTestCreationResult(string message)
    {
        List<ValidationError> errors = new() { new(message) };

        return CreateFailedTestCreationResult(errors);
    }
    
    private static object?[][] CreateFailedTestCreationResult(List<ValidationError> errors)
    {
        return new[]
        {
            new object?[]
            {
                errors,
                null,
                null,
                0
            }
        };
    }
    
    private static object?[] CreateSingleTest(List<ValidationError> errors, Chip? chip, TestData? testData, int order)
    {
        return new object?[]
        {
            errors,
            chip,
            testData,
            order
        };
    }
}