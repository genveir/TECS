using FluentAssertions;
using NUnit.Framework;
using TECS.HDLSimulator.Chips;

namespace TECS.Tests;

public class TestRunner
{
    [TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.Create), new object?[] {Settings.DataFolder})]
    public void RunTest(string name, Chip chip, TestFile testFile, ComparisonFile comparisonFile)
    {
        var lines = testFile.Lines;
        var index = 10;
        
        int comparisonLine = 1;
        while (index < lines.Length && !string.IsNullOrWhiteSpace(lines[index]))
        {
            RunTest(lines, ref index, chip);
            
            CheckTest(comparisonFile.Lines[comparisonLine++], lines[8], chip);
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

                chip.Inputs[inputToSet].Value = value;
            }

            index++;
        } while (!string.IsNullOrWhiteSpace(lines[index]));
    }

    private void CheckTest(string comparisonLine, string outputList, Chip chip)
    {
        var pinsToCheck = outputList
            .Split(new[] { ' ', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(s => s.Substring(0, s.IndexOf('%')))
            .ToArray();

        var comparisonData = comparisonLine
            .Split('|', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => s.Trim())
            .Select(s => s.Select(c => c == '1'))
            .ToArray();
        
        var allValues = new Dictionary<string, bool[]>();
        foreach (var input in chip.Inputs)
            allValues.Add(input.Key, input.Value.Value);

        foreach (var output in chip.Outputs)
            allValues.Add(output.Key, output.Value.Value);

        pinsToCheck.Length.Should().Be(comparisonData.Length);
        for (int n = 0; n < pinsToCheck.Length; n++)
        {
            allValues[pinsToCheck[n]].Should().BeEquivalentTo(comparisonData[n]);
        }
    }
}