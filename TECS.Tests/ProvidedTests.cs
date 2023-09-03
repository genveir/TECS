using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Names;
using TECS.DataIntermediates.Test;
using TECS.DataIntermediates.Values;
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
    [ProvidedTest("AluCombiner")]
    [ProvidedTest("AluPreset")]
    [ProvidedTest("FullAdder")]
    [ProvidedTest("HalfAdder")]
    [ProvidedTest("Inc16")]
    [ProvidedTest("Neg16")]
    public void Chapter2(List<ValidationError> errors, Chip? chip, TestData? testData) =>
        RunTests(errors, chip, testData);
    
    [ProvidedTest("Latch")]
    [ProvidedTest("DFF")]
    public void Chapter3Custom(List<ValidationError> errors, Chip? chip, TestData? testData) =>
        RunTests(errors, chip, testData);
    
    [ProvidedTest("Bit")]
    [ProvidedTest("PC")]
    [ProvidedTest("RAM8")]
    [ProvidedTest("RAM64")]
    [ProvidedTest("Register")]
    public void Chapter3A(List<ValidationError> errors, Chip? chip, TestData? testData) =>
        RunTests(errors, chip, testData);
    
    [ProvidedTest("RAM4K")]
    [ProvidedTest("RAM16K")]
    [ProvidedTest("RAM512")]
    public void Chapter3B(List<ValidationError> errors, Chip? chip, TestData? testData) =>
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
            var columnsToCheck = testData.ExpectedValues.ColumnsToCheck;
            var valuesToCheck = testData.ExpectedValues.Values[n];
            
            RunTest(chip, ordered[n], columnsToCheck, valuesToCheck);
        }
    }

    private void RunTest(Chip chip, TestInputData inputs, ColumnData[] columnsToCheck, string[] valuesToCheck)
    {
        foreach (var setData in inputs.SetData)
        {
            var group = setData.Group;
            var value = setData.ValueToSet;

            try
            {
                chip.SetInput(group, value);
            }
            catch (Exception e)
            {
                Assert.Fail(e.Message);
            }
        }

        var result = chip.Evaluate();
        
        var allPinValues = new Dictionary<NamedNodeGroupName, BitValue>();
        foreach (var input in result.InputValues)
            allPinValues.Add(input.Key, input.Value);

        foreach (var output in result.OutputValues)
            allPinValues.Add(output.Key, output.Value);

        for (int n = 0; n < columnsToCheck.Length; n++)
        {
            var columnToCheck = columnsToCheck[n];
            var expectedValue = valuesToCheck[n];

            switch (columnToCheck.Type)
            {
                case ColumnType.BinaryString:
                    allPinValues[columnToCheck.Name].FormatForOutput().Should().Be(expectedValue);
                    break;
                case ColumnType.Number:
                    allPinValues[columnToCheck.Name].AsLongValue().FormatForOutput().Should().Be(expectedValue);
                    break;
                case ColumnType.Clock:
                    chip.Clock.Should().Be(expectedValue == "1");
                    break;
                case ColumnType.Time:
                    chip.Time.Should().Be(expectedValue);
                    break;
            }
        }
    }

    private void CheckBinaryStringValue(Dictionary<NamedNodeGroupName, BitValue> allPinValues,
        ColumnData columnData, string expected)
    {
        
    }
}