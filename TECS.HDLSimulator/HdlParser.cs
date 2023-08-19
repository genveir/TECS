using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualBasic.CompilerServices;
using TECS.HDLSimulator.Chips;

namespace TECS.HDLSimulator;

public static class HdlParser
{
    public static ChipSummary ParseSummary(string input)
    {
        var lines = SplitAndSanitizeInput(input);

        int lineIndex = 0;
        return ParseChipSummary(lines, ref lineIndex);
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

    private static ChipSummary ParseChipSummary(string[] lines, ref int lineIndex)
    {
        // CHIP name {
        var split = lines[lineIndex++].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = split[1];

        List<PinSummary> IN = ParsePinArray(lines, ref lineIndex);
        List<PinSummary> OUT = ParsePinArray(lines, ref lineIndex);
        List<PartSummary> PARTS = ParsePartsSummary(lines, ref lineIndex);

        return new(name, IN, OUT, PARTS);
    }

    private static List<PinSummary> ParsePinArray(string[] lines, ref int lineIndex)
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
            .Select(ParsePinSummary)
            .ToList();
    }

    private static PinSummary ParsePinSummary(string pinData)
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

    private static List<PartSummary> ParsePartsSummary(string[] lines, ref int lineIndex)
    {
        List<PartSummary> parts = new();

        while (true)
        {
            if (lines[lineIndex].Contains("PARTS")) lineIndex++;
            if (lines[lineIndex].Contains('}')) return parts;

            parts.Add(ParsePartSummary(lines, ref lineIndex));
        }

    }

    private static PartSummary ParsePartSummary(string[] lines, ref int lineIndex) {
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
        List<PinConnectionSummary> pinConnections = new();
        
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