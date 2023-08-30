using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.Tests;

// ReSharper disable NotResolvedInText
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

    private class CustomTestAttribute : TestCaseSourceAttribute
    {
        public CustomTestAttribute(string name) : base(
            typeof(TestDataFactory), nameof(TestDataFactory.EntryPoint),
            new object?[] { Settings.Test2Folder, name })
        {
        }
    }
    
    [ProvidedTest("And")]
    [ProvidedTest("And16")]
    [ProvidedTest("DMux")]
    [ProvidedTest("DMux4Way")]
    [ProvidedTest("DMux8Way")]
    [ProvidedTest("Mux")]
    [ProvidedTest("Not")]
    [ProvidedTest("Not16")]
    [ProvidedTest("Or")]
    [ProvidedTest("Or8Way")]
    [ProvidedTest("Or16")]
    [ProvidedTest("Xor")]
    public void Chapter1(List<ValidationError> errors, Chip? chip, TestData? testData) =>
        RunTests(errors, chip, testData);
    
    [ProvidedTest("Add16")]
    [ProvidedTest("ALU")]
    [ProvidedTest("ALU-nostat")]
    [ProvidedTest("AluPreset")]
    [ProvidedTest("FullAdder")]
    [ProvidedTest("HalfAdder")]
    [ProvidedTest("Inc16")]
    [ProvidedTest("Neg16")]
    public void Chapter2(List<ValidationError> errors, Chip? chip, TestData? testData) =>
        RunTests(errors, chip, testData);
    
    private void RunTests(List<ValidationError> errors, Chip? chip, TestData? testData)
    {
        errors.Should().BeEmpty();

        chip.Should().NotBeNull();
        testData.Should().NotBeNull();
        if (chip == null) return;
        if (testData == null) return;

        var inputs = testData.Tests;

        var ordered = inputs.OrderBy(i => i.Order).ToArray();

        for (int n = 0; n < inputs.Length; n++)
        {
            var groupsToCheck = testData.ExpectedValues.GroupsToCheck;
            var valuesToCheck = testData.ExpectedValues.Values[n];
            
            RunTest(chip, ordered[n], groupsToCheck, valuesToCheck);
        }
    }

    private void RunTest(Chip chip, TestInputData inputs, NamedNodeGroupName[] groupsToCheck, BitValue[] valuesToCheck)
    {
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

        var allValues = new Dictionary<NamedNodeGroupName, BitValue>();
        foreach (var input in chip.Inputs)
            allValues.Add(input.Key, input.Value.GetValue());

        foreach (var output in chip.Outputs)
            allValues.Add(output.Key, output.Value.GetValue());

        for (int n = 0; n < groupsToCheck.Length; n++)
        {
            var groupToCheck = groupsToCheck[n];
            var expectedValue = valuesToCheck[n];

            var actualValue = allValues[groupToCheck];

            actualValue.Should().BeEquivalentTo(expectedValue);
        }
    }
}