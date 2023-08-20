using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using TECS.HDLSimulator.Chips;

namespace TECS.HDLSimulator;

public static class HdlParser
{
    public static ChipDescription ParseDescription(string input)
    {
        var lines = SplitAndSanitizeInput(input);

        int lineIndex = 0;
        return ParseChipDescription(lines, ref lineIndex);
    }

    private static string[] SplitAndSanitizeInput(string input)
    {
        var lines = input
            .Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Where(l => !l.Contains("//"))
            .Where(l => !l.Contains("*"))
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .Select(l => l.Trim())
            .ToArray();

        return lines;
    }

    private static ChipDescription ParseChipDescription(string[] lines, ref int lineIndex)
    {
        // CHIP name {
        var split = lines[lineIndex++].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = split[1];

        List<PinDescription> IN = ParsePinArray(lines, ref lineIndex);
        List<PinDescription> OUT = ParsePinArray(lines, ref lineIndex);
        List<PartDescription> PARTS = ParsePartsDescription(lines, ref lineIndex);

        return new(name, IN, OUT, PARTS);
    }

    private static List<PinDescription> ParsePinArray(string[] lines, ref int lineIndex)
    {
        string pinData = "";
        string newLine = "";
        do
        {
            newLine = lines[lineIndex++];

            pinData = pinData + newLine;
        } while (!newLine.EndsWith(';'));

        return pinData
            .Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
            .Skip(1)
            .Select(ParsePinDescription)
            .ToList();
    }

    private static PinDescription ParsePinDescription(string pinData)
    {
        if (pinData.Contains('['))
        {
            var splitData = pinData.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            var name = splitData[0];
            var bitSize = int.Parse(splitData[1]);
                    
            return new(name, bitSize);
        }
        else
        {
            return new(pinData, 1);
        }
    }

    private static List<PartDescription> ParsePartsDescription(string[] lines, ref int lineIndex)
    {
        List<PartDescription> parts = new();

        while (true)
        {
            if (lines[lineIndex].Contains("PARTS")) lineIndex++;
            if (lines[lineIndex].Contains('}')) return parts;

            parts.Add(ParsePartDescription(lines, ref lineIndex));
        }

    }

    private static PartDescription ParsePartDescription(string[] lines, ref int lineIndex) {
        string partData = "";
        string newLine = "";
        do
        {
            newLine = lines[lineIndex++];

            partData = partData + newLine;
        } while (!newLine.EndsWith(';'));

        var splitData = partData
            .Split(new[] { '(', ')', ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

        var name = splitData[0];
        List<PinConnectionDescription> pinConnections = new();
        
        for (int n = 1; n < splitData.Length; n++)
        {
            var splitPinConnection = splitData[n].Split('=');
            var internalPin = splitPinConnection[0];
            var externalPin = splitPinConnection[1];
            pinConnections.Add(new(internalPin, externalPin));
        }

        return new(pinConnections, name);
    }
}