using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Test;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public class TstToIntermediateMapper
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
            
            var builder = new TestDataBuilder();

            MapChipToTest(hdlFolder, builder, lines, ref index);
            MapExpectedValues(hdlFolder, builder, lines, ref index);
            MapOutputList(builder, lines, ref index);
            MapTests(builder, lines, ref index);

            testData = builder.Build();
            Mapped[file] = testData;
        }

        return testData;
    }

    private static void MapChipToTest(HdlFolder folder, TestDataBuilder builder, string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("load")))
            throw new MappingException("Test File contains no load instruction");

        var split = lines[index].Split(new[] { ' ', '.' });

        var hdl = folder.HdlFiles.SingleOrDefault(hdl => hdl.Name == split[1]);
        if (hdl == null)
            throw new MappingException("can not find HDL file to run tests against");

        var chipData = HdlToIntermediateMapper.Map(hdl);

        builder.WithChipToTest(chipData);
    }

    private static void MapExpectedValues(HdlFolder folder, TestDataBuilder builder, string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("compare-to")))
            throw new MappingException("Test File contains no compare-to instruction");

        var split = lines[index].Split(new[] { ' ', '.' });

        var cmp = folder.ComparisonFiles.SingleOrDefault(cmp => cmp.Name == split[1]);
        if (cmp == null)
            throw new MappingException("can not find CMP file to run tests against");

        var cmpData = StringArraySanitizer.SanitizeInput(cmp.GetContents())
            .Select(l => 
                l.Split('|', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .ToArray())
            .ToArray();

        var expectedBuilder = builder.SetExpectedValues()
            .WithGroups(cmpData[0]);

        for (int n = 1; n < cmpData.Length; n++)
            expectedBuilder.AddValueRow(cmpData[n]);

        expectedBuilder.Build();
    }
    
    private static void MapOutputList(TestDataBuilder builder, string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("output-list")))
            throw new MappingException("Test file contains no output list");

        var split = lines[index].Split(' ', StringSplitOptions.RemoveEmptyEntries);

        for (int n = 1; n < split.Length; n++)
        {
            var setterSplit = split[n].Split(new[] { '%', '.' });

            builder.AddOutput(setterSplit[0], int.Parse(setterSplit[2]));
        }
    }

    private static void MapTests(TestDataBuilder builder, string[] lines, ref int index)
    {
        int order = 0;

        while (StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("set")))
        {
            MapTest(builder, lines, ref index, order++);
        }
    }

    private static void MapTest(TestDataBuilder builder, string[] lines, ref int index, int order)
    {
        var testBuilder = builder.AddTest(order);

        while (lines[index].StartsWith("set"))
        {
            var splitSetter = lines[index]
                .Replace("%B", "")
                .Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);

            testBuilder.AddInput(splitSetter[1], splitSetter[2]);
            
            index++;
        }

        testBuilder.Build();
    }
    
    
}