using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Test;
using TECS.FileAccess;
using TECS.FileAccess.FileAccessors;
using TECS.FileAccess.Mappers;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.Tests;

public static class TestDataFactory
{
    private static IChipBluePrintFactory? _factory;

    private static IChipBluePrintFactory GetFactory(string dataFolder)
    {
        if (_factory == null)
            _factory = new VeryTemporaryFactory();
        
        return _factory;
    }

    public static IEnumerable<TestCaseData> EntryPoint(string dataFolder, string name)
    {
        foreach (var testCaseData in Create(dataFolder, name))
            yield return testCaseData;
    }

    private static IEnumerable<TestCaseData> Create(string dataFolder, string name)
    {
        var hdlFolder = new DataFolder(dataFolder).HdlFolder;

        var factory = GetFactory(dataFolder);

        var testFile = hdlFolder.TestFiles.SingleOrDefault(tst => tst.Name == name);
        if (testFile == null)
            yield return CreateFailedTestCreationResult(name, $"could not find test file {name}");
        else
            foreach (var testCaseData in Create(hdlFolder, factory, testFile, name))
                yield return testCaseData;
    }

    private static IEnumerable<TestCaseData> Create(HdlFolder hdlFolder, IChipBluePrintFactory factory,
        TestFile testFile, string name)
    {
        TestData? testData = null;
        Exception? thrown = null;
        try
        {
            testData = TstToIntermediateMapper.Map(hdlFolder, testFile);
        }
        catch (Exception e)
        {
            thrown = e;
        }

        if (thrown != null)
            yield return CreateFailedTestCreationResult(name, thrown.Message);
        else
        {
            if (testData == null) yield break;
            foreach (var testCaseData in Create(factory, testData, name))
                yield return testCaseData;
        }
    }

    private static IEnumerable<TestCaseData> Create(IChipBluePrintFactory factory, TestData testData, string name)
    {
        var storedBlueprint = factory.CreateBlueprint(testData.ChipToTest);
        if (storedBlueprint.ValidationErrors.Any())
            yield return CreateFailedTestCreationResult(name, storedBlueprint.ValidationErrors);
        else
            foreach (var testCaseData in Create(storedBlueprint, testData, name))
                yield return testCaseData;
    }

    private static IEnumerable<TestCaseData> Create(StoredBlueprint blueprint, TestData testData, string name)
    {
        var chip = blueprint.CopyToBlueprintInstance().Fabricate();

        for (int n = 0; n < testData.Tests.Length; n++)
        {
            yield return CreateSingleTest($"{name}_{n}", new(), chip, testData, n);
        }
    }

    private static TestCaseData CreateFailedTestCreationResult(string name, string message)
    {
        List<ValidationError> errors = new() { new(message) };

        return CreateFailedTestCreationResult(name, errors);
    }

    private static TestCaseData CreateFailedTestCreationResult(string name, List<ValidationError> errors)
    {
        return
            new TestCaseData(
                errors,
                null,
                null,
                0).SetName(name);
    }

    private static TestCaseData CreateSingleTest(string name, List<ValidationError> errors, Chip? chip,
        TestData? testData, int order)
    {
        return
            new TestCaseData(
                errors,
                chip,
                testData,
                order).SetName(name);
    }
}