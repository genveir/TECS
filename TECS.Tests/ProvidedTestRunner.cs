using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.FileAccess.FileAccessors;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.Tests;

public class ProvidedTestRunner
{
    [TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.Create), new object?[] {Settings.DataFolder})]
    public void RunProvidedTests(string name, List<ValidationError> errors, Chip? chip, TestFile testFile, ComparisonFile comparisonFile)
    {
        errors.Should().BeEmpty();

        chip.Should().NotBeNull();
        if (chip == null) return;
        
        var lines = testFile.GetContents();
        var index = ResolveLineNumberOfFirstSet(testFile);

        var outputList = ResolveOutputList(testFile);
        
        int comparisonLine = 1;
        while (index < lines.Length && !string.IsNullOrWhiteSpace(lines[index]))
        {
            var hasTest = RunTest(lines, ref index, chip);

            if (hasTest)
                CheckTest(comparisonFile.GetContents()[comparisonLine++], outputList, chip);
        }
    }

    private string ResolveOutputList(TestFile testFile)
    {
        var lines = testFile.GetContents();
        
        string outputList = "";
        bool atList = false;
        for (int n = 0; n < lines.Length; n++)
        {
            var line = lines[n];
            
            if (line.StartsWith("output-list"))
            {
                atList = true;
            }

            if (atList)
            {
                outputList += line;
                
                if (line.Contains(';')) break;
            }
        }

        return outputList;
    }

    private int ResolveLineNumberOfFirstSet(TestFile testFile)
    {
        int lineCounter = 0;

        while (true)
        {
            if (testFile.GetContents()[lineCounter].StartsWith("set")) return lineCounter;
            lineCounter++;
        }
    }
    
    private bool RunTest(string[] lines, ref int index, Chip chip)
    {
        bool hasTest = false;
        do
        {
            if (lines[index].StartsWith("set"))
            {
                hasTest = true;
                var split = lines[index].Split(new[] { ' ', ',', '%', 'B' }, StringSplitOptions.RemoveEmptyEntries);
                var inputToSet = split[1];
                var value = split[2].Select(c => c == '1').ToArray();

                for (int n = 0; n < value.Length; n++)
                    chip.Inputs[inputToSet].Nodes[n].Value = value[n];
            }

            index++;
        } while (index < lines.Length && !lines[index].Contains("output"));

        return hasTest;
    }

    private void CheckTest(string comparisonLine, string outputList, Chip chip)
    {
        var pinGroupsToCheck = outputList
            .Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(s => s.Substring(0, s.IndexOf('%')))
            .ToArray();

        var comparisonData = comparisonLine
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Select(s => s.Select(c => c == '1').ToArray())
            .ToArray();
        
        var allValues = new Dictionary<string, bool[]>();
        foreach (var input in chip.Inputs)
            allValues.Add(input.Key, input.Value.Value);

        foreach (var output in chip.Outputs)
            allValues.Add(output.Key, output.Value.Value);

        pinGroupsToCheck.Length.Should().Be(comparisonData.Length);
        for (int n = 0; n < pinGroupsToCheck.Length; n++)
        {
            allValues[pinGroupsToCheck[n]].Should().BeEquivalentTo(comparisonData[n]);
        }
    }
}