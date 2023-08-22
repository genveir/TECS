using System;
using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.HDL;

public static class HdlParser
{
    public static ChipDescription ParseDescription(string input)
    {
        var lines = SplitAndSanitizeInput(input);

        var lineIndex = 0;
        return ParseChipDescription(lines, ref lineIndex);
    }

    private static string[] SplitAndSanitizeInput(string input)
    {
        var lines = input
            .Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(l => l.Trim())
            .Select(StripComments)
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

        return lines;
    }

    private static string StripComments(string line)
    {
        if (line.Contains('*')) return "";

        return line.Contains("//") ? line[..line.IndexOf('/')].Trim() : line;
    }

    private static ChipDescription ParseChipDescription(string[] lines, ref int lineIndex)
    {
        // CHIP name {
        var split = lines[lineIndex++].Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var name = split[1];

        List<NamedPinGroupDescription> inGroups = ParsePinGroupArray(lines, ref lineIndex);
        List<NamedPinGroupDescription> outGroups = ParsePinGroupArray(lines, ref lineIndex);
        List<PartDescription> parts = ParsePartsDescription(lines, ref lineIndex);

        return new(name, inGroups, outGroups, parts);
    }

    private static List<NamedPinGroupDescription> ParsePinGroupArray(string[] lines, ref int lineIndex)
    {
        string pinData = "";
        string newLine;
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

    private static NamedPinGroupDescription ParsePinDescription(string pinData)
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
        string newLine;
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
            var internalLink = ParsePinLinkGroup(splitPinConnection[0]);
            var externalLink = ParsePinLinkGroup(splitPinConnection[1]);
            pinConnections.Add(new(internalLink, externalLink));
        }

        return new(pinConnections, name);
    }

    private static PinLinkGroupDescription ParsePinLinkGroup(string stringRepresentation)
    {
        var split = stringRepresentation.Split(new[] { ' ', '[', ']', '.' }, StringSplitOptions.RemoveEmptyEntries);

        return split.Length switch
        {
            1 => new(split[0], null, null),
            2 => new(split[0], int.Parse(split[1]), int.Parse(split[1])),
            3 => new(split[0], int.Parse(split[1]), int.Parse(split[2])),
            _ => throw new InvalidOperationException("link group is not defined")
        };
    }
}