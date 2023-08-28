using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;
using TECS.Tests.Intermediates.Test;

namespace TECS.Tests;

public class ProvidedTests
{
    private class ProvidedTestAttribute : TestCaseSourceAttribute
    {
        public ProvidedTestAttribute(string name) 
            : base(
                typeof(TestDataFactory), nameof(TestDataFactory.EntryPoint),
                new object?[] { Settings.DataFolder, name }) 
        {
            
        }
    }

    [ProvidedTest("Add16")]
    [ProvidedTest("ALU")]
    [ProvidedTest("ALU-nostat")]
    [ProvidedTest("AluPreset")]
    [ProvidedTest("And")]
    [ProvidedTest("And16")]
    [ProvidedTest("DMux")]
    [ProvidedTest("DMux4Way")]
    [ProvidedTest("DMux8Way")]
    [ProvidedTest("FullAdder")]
    [ProvidedTest("HalfAdder")]
    [ProvidedTest("Inc16")]
    [ProvidedTest("Mux")]
    [ProvidedTest("Neg16")]
    [ProvidedTest("Not")]
    [ProvidedTest("Not16")]
    [ProvidedTest("Or")]
    [ProvidedTest("Or8Way")]
    [ProvidedTest("Or16")]
    [ProvidedTest("Xor")]
    public void RunTest(List<ValidationError> errors, Chip? chip, TestData? testData, int order)
    {
        errors.Should().BeEmpty(because: $"{errors.Count} errors: errors.First().Message [...]");

        chip.Should().NotBeNull();
        testData.Should().NotBeNull();
        if (chip == null) return;
        if (testData == null) return;

        var inputs = testData.Tests.Single(t => t.Order == order);
        
        foreach (var setData in inputs.SetData)
        {
            var group = setData.Group;
            var value = setData.ValueToSet;

            try
            {
                chip.Inputs[group].SetValue(value);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }
        
        var allValues = new Dictionary<NamedNodeGroupName, bool[]>();
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

            var actualValue = allValues[groupToCheck];

            actualValue.Should().BeEquivalentTo(expectedValue.Value);
        }
    }
}