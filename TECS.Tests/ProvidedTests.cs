using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.FileAccess.FileAccessors;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.Tests;

public class ProvidedTests
{
    [TestCaseSource(typeof(TestDataFactory), nameof(TestDataFactory.Create), new object?[] {Settings.DataFolder, "Not"})]
    public void Not(List<ValidationError> errors, Chip? chip, TestData? testData, int order)
    {
        errors.Should().BeEmpty();

        chip.Should().NotBeNull();
        testData.Should().NotBeNull();
        if (chip == null) return;
        if (testData == null) return;

        var inputs = testData.Tests.Single(t => t.Order == order);
        
        foreach (var setData in inputs.SetData)
        {
            var group = setData.Group.Value;
            var value = setData.ValueToSet.Value;

            for (int n = 0; n < value.Length; n++)
                chip.Inputs[group].Nodes[n].Value = value[n];
        }
        
        var allValues = new Dictionary<string, bool[]>();
        foreach (var input in chip.Inputs)
            allValues.Add(input.Key, input.Value.Value);
        
        foreach (var output in chip.Outputs)
            allValues.Add(output.Key, output.Value.Value);

        var groupsToCheck = testData.ExpectedValues.GroupsToCheck;
        var valuesToCheck = testData.ExpectedValues.Values[order];

        for (int n = 0; n < groupsToCheck.Length; n++)
        {
            var groupToCheck = groupsToCheck[n];
            var expectedValue = valuesToCheck[n];

            var actualValue = allValues[groupToCheck.Value];

            actualValue.Should().BeEquivalentTo(expectedValue.Value);
        }
    }
}