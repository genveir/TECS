using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public static class HdlToIntermediateMapper
{
    private static readonly Dictionary<HdlFile, ChipData> Mapped = new();

    public static ChipData Map(HdlFile file)
    {
        if (!Mapped.TryGetValue(file, out var chipData))
        {
            var lines = file.GetContents();

            lines = StringArraySanitizer.SanitizeInput(lines);
            var index = 0;

            var chipName = MapChipName(lines, ref index);
            var inGroups = MapIn(lines, ref index);
            var outGroups = MapOut(lines, ref index);
            var parts = MapParts(lines, ref index);

            chipData = new ChipDataBuilder()
                .WithName(chipName)
                .WithInGroups(inGroups)
                .WithOutGroups(outGroups)
                .WithParts(parts)
                .Build();
            
            Mapped[file] = chipData;
        }

        return chipData;
    }

    private static ChipName MapChipName(string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("CHIP")))
            throw new MappingException("HDL file has no defined chip");
        
        var split = lines[index].Split(' ');

        return new ChipName(split[1]);
    }

    private static IEnumerable<NamedNodeGroupData> MapIn(string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("IN")) ||
            !StringArrayNavigator.GrabArray(lines, ref index, out string[] elements))
        {
            throw new MappingException("HDL file has no defined inputs");
        }

        return MapGroups(elements);
    }

    private static IEnumerable<NamedNodeGroupData> MapOut(string[] lines, ref int index)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("OUT")) ||
            !StringArrayNavigator.GrabArray(lines, ref index, out string[] elements))
        {
            throw new MappingException("HDL file has no defined outputs");
        }

        return MapGroups(elements);
    }

    private static IEnumerable<NamedNodeGroupData> MapGroups(string[] elements)
    {
        List<NamedNodeGroupData> groups = new();
        foreach (var element in elements)
        {
            var (name, size) = MapGroup(element);
            
            groups.Add(new(new(name), new(size)));
        }

        return groups;
    }

    private static (string name, int size) MapGroup(string element)
    {
        var elementSplit = element.Split(new[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);

        return elementSplit.Length switch
        {
            1 => (elementSplit[0], 1),
            2 => (elementSplit[0], int.Parse(elementSplit[1])),
            _ => throw new MappingException($"Group (element) can not be parsed")
        };
    }

    private static IEnumerable<ChipPartData> MapParts(string[] lines, ref int index)
    {
        List<ChipPartData> parts = new();
        if (StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("PARTS:")))
        {
            string fullPartArray = "";

            while (index < lines.Length)
                fullPartArray += lines[index++];

            fullPartArray = fullPartArray
                .Replace("PARTS:", "")
                .Replace(" ", "");

            var splitPartArray = fullPartArray.Split(';', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var partInfo in splitPartArray)
            {
                parts.Add(MapPart(partInfo));
            }
        }

        return parts;
    }

    private static ChipPartData MapPart(string partInfo)
    {
        var splitPartInfo = partInfo.Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

        var partBuilder = new SimpleChipPartDataBuilder(splitPartInfo[0]);

        var links = splitPartInfo.Skip(1).ToArray();

        foreach (var link in links)
        {
            MapLink(link, partBuilder);
        }

        return partBuilder.Build();
    }

    private static void MapLink(string linkInfo, SimpleChipPartDataBuilder builder)
    {
        var splitLink = linkInfo.Split('=');

        var linkBuilder = builder.AddLink();

        var (name, lowerBound, upperBound) = ParseLink(splitLink[0]);
        linkBuilder.WithInternal(name, lowerBound, upperBound);
        
        (name, lowerBound, upperBound) = ParseLink(splitLink[1]);
        linkBuilder.WithExternal(name, lowerBound, upperBound);

        linkBuilder.Build();
    }

    private static (string name, int? lowerBound, int? upperBound) ParseLink(string link)
    {
        var splitInfo = link.Split(new[] { '[', '.', ']' }, StringSplitOptions.RemoveEmptyEntries);

        return splitInfo.Length switch
        {
            1 => (splitInfo[0], null, null),
            2 => (splitInfo[0], int.Parse(splitInfo[1]), int.Parse(splitInfo[1])),
            3 => (splitInfo[0], int.Parse(splitInfo[1]), int.Parse(splitInfo[2])),
            _ => throw new MappingException("link {link} is not formatted correctly")
        };
    }
}