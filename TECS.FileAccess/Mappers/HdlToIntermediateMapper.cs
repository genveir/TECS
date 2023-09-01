using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;
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

            var builder = new ChipDataBuilder();

            MapChipName(lines, ref index, builder);
            MapIn(lines, ref index, builder);
            MapOut(lines, ref index, builder);
            MapParts(lines, ref index, builder);

            chipData = builder.Build();
            Mapped[file] = chipData;
        }

        return chipData;
    }

    private static void MapChipName(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("CHIP")))
            throw new MappingException("HDL file has no defined chip");
        
        var split = lines[index].Split(' ');

        builder.WithName(split[1]);
    }

    private static void MapIn(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("IN")) ||
            !StringArrayNavigator.GrabArray(lines, ref index, out string[] elements))
        {
            throw new MappingException("HDL file has no defined inputs");
        }

        foreach (var element in elements)
        {
            var (name, size) = MapGroup(element);

            builder.AddInGroup(name, size);
        }
    }

    private static void MapOut(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (!StringArrayNavigator.LoopForwardTo(lines, ref index, l => l.StartsWith("OUT")) ||
            !StringArrayNavigator.GrabArray(lines, ref index, out string[] elements))
        {
            throw new MappingException("HDL file has no defined outputs");
        }

        foreach (var element in elements)
        {
            var (name, size) = MapGroup(element);

            builder.AddOutGroup(name, size);
        }
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

    private static void MapParts(string[] lines, ref int index, ChipDataBuilder builder)
    {
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
                MapPart(partInfo, builder);
            }
        }
    }

    private static void MapPart(string partInfo, ChipDataBuilder builder)
    {
        var splitPartInfo = partInfo.Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

        var partBuilder = builder.AddPart(splitPartInfo[0]);

        var links = splitPartInfo.Skip(1).ToArray();

        foreach (var link in links)
        {
            MapLink(link, partBuilder);
        }

        partBuilder.Build();
    }

    private static void MapLink(string linkInfo, ChipPartDataBuilder<ChipDataBuilder> builder)
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