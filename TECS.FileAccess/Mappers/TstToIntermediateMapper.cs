using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public static class TstToIntermediateMapper
{
    private static readonly Dictionary<TestFile, TestData> Mapped = new();

    public static TestData Map(HdlFolder hdlFolder, TestFile file)
    {
        if (!Mapped.TryGetValue(file, out var testData))
        {
            var lines = file.GetContents();

            lines = StringArraySanitizer.SanitizeInput(lines);
            lines = StringArraySanitizer.UnifyTestLines(lines);
            var index = 0;

            var chip = GetChipToTest(hdlFolder, lines, ref index);
            var comparisonFile = GetComparisonFile(hdlFolder, lines, ref index);
            var outputTypes = MapOutputList(lines, ref index);
            var expectedValues = MapExpectedValues(comparisonFile, outputTypes);
            var tests = MapTests(lines, ref index);

            testData = new TestDataBuilder()
                .WithChipToTest(chip)
                .WithExpectedValues(expectedValues)
                .WithTests(tests)
                .Build();

            Mapped[file] = testData;
        }

        return testData;
    }

    private static ChipData GetChipToTest(HdlFolder folder, string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("load")))
            throw new MappingException("Test File contains no load instruction");

        var split = lines[index].Split(new[] { ' ', '.' });

        var hdl = folder.HdlFiles.SingleOrDefault(hdl => hdl.Name == split[1]);
        if (hdl == null)
            throw new MappingException("can not find HDL file to run tests against");

        var chipData = HdlToIntermediateMapper.Map(hdl);

        return chipData;
    }

    private static ComparisonFile GetComparisonFile(HdlFolder folder, string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("compare-to")))
            throw new MappingException("Test File contains no compare-to instruction");

        var split = lines[index].Split(new[] { ' ', '.' });

        var cmp = folder.ComparisonFiles.SingleOrDefault(cmp => cmp.Name == split[1]);
        if (cmp == null)
            throw new MappingException("can not find CMP file to run tests against");

        return cmp;
    }

    private static ColumnData[] MapOutputList(string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("output-list")))
            throw new MappingException("Test file contains no output list");

        var split = lines[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        List<ColumnData> columns = new();
        for (int n = 1; n < split.Length; n++)
        {
            var setterSplit = split[n].Split(new[] { '%', '.' });

            var name = setterSplit[0];

            ColumnType type;
            if (name == "time") type = ColumnType.Time;
            else if (name == "clock") type = ColumnType.Clock;
            else if (setterSplit[1].StartsWith("B")) type = ColumnType.BinaryString;
            else type = ColumnType.Number;

            var nodeGroupName = new NamedNodeGroupName(name);
            columns.Add(new(nodeGroupName, type));
        }

        return columns.ToArray();
    }

    private static CompareData MapExpectedValues(ComparisonFile cmp, ColumnData[] columns)
    {
        var cmpData = StringArraySanitizer.SanitizeInput(cmp.GetContents())
            .Select(l =>
                l.Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray())
            .ToArray();

        var expectedBuilder = new SimpleCompareDataBuilder();
        for (int n = 0; n < cmpData[0].Length; n++)
        {
            if (!columns[n].Name.Equals(new NamedNodeGroupName(cmpData[0][n])))
            {
                throw new MappingException($"Mismatch in columns between {columns[n].Name} and {cmpData[0][n]}");
            }

            expectedBuilder.AddColumn(columns[n]);
        }

        for (int n = 1; n < cmpData.Length; n++)
            expectedBuilder.AddValueRow(cmpData[n]);

        return expectedBuilder.Build();
    }

    private static IEnumerable<TestInputData> MapTests(string[] lines, ref int index)
    {
        int order = 0;

        List<TestInputData> tests = new();
        while (StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("set") || l.StartsWith("tock")))
        {
            tests.Add(MapTest(lines, ref index, order++));
        }

        return tests;
    }

    private static TestInputData MapTest(string[] lines, ref int index, int order)
    {
        var testBuilder = new SimpleTestInputDataBuilder(order);
        while (lines[index].StartsWith("set"))
        {
            var splitSetter = lines[index]
                .Replace("%B", "")
                .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            testBuilder.AddInput(splitSetter[1], splitSetter[2]);

            index++;
        }

        index++;

        return testBuilder.Build();
    }
}