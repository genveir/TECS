using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
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
        
        var lines = testFile.Lines;
        var index = ResolveLineNumberOfFirstSet(testFile);

        var outputList = ResolveOutputList(testFile);
        
        int comparisonLine = 1;
        while (index < lines.Length && !string.IsNullOrWhiteSpace(lines[index]))
        {
            RunTest(lines, ref index, chip);

            CheckTest(comparisonFile.Lines[comparisonLine++], outputList, chip);
        }
    }

    private string ResolveOutputList(TestFile testFile)
    {
        string outputList = "";
        bool atList = false;
        for (int n = 0; n < testFile.Lines.Length; n++)
        {
            var line = testFile.Lines[n];
            
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
            if (testFile.Lines[lineCounter].StartsWith("set")) return lineCounter;
            lineCounter++;
        }
    }
    
    private void RunTest(string[] lines, ref int index, Chip chip)
    {
        do
        {
            if (lines[index].StartsWith("set"))
            {
                var split = lines[index].Split(new[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
                var inputToSet = split[1];
                var value = split[2].Select(c => c == '1').ToArray();

                chip.Inputs[inputToSet].Nodes[0].Value = value[0];
            }

            index++;
        } while (!string.IsNullOrWhiteSpace(lines[index]));
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