using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Chip;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public static class HdlToIntermediateMapper
{
    public static ChipData Map(HdlFile file)
    {
        var lines = file.GetContents();

        lines = SanitizeInput(lines);
        var index = 0;

        var builder = new ChipDataBuilder();

        MapChipName(lines, ref index, builder);
        MapIn(lines, ref index, builder);
        MapOut(lines, ref index, builder);
        MapParts(lines, ref index, builder);

        return builder.Build();
    }

    private static void MapChipName(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (!LoopForwardTo(lines, ref index, l => l.StartsWith("CHIP")))
            throw new MappingException("HDL file has no defined chip");
        
        var split = lines[index].Split(' ');

        builder.WithName(split[1]);
    }

    private static void MapIn(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (!LoopForwardTo(lines, ref index, l => l.StartsWith("IN")) ||
            !GrabArray(lines, ref index, out string[] elements))
        {
            throw new MappingException("HDL file has no defined inputs");
        }

        builder.WithInGroups(elements);
    }

    private static void MapOut(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (!LoopForwardTo(lines, ref index, l => l.StartsWith("OUT")) ||
            !GrabArray(lines, ref index, out string[] elements))
        {
            throw new MappingException("HDL file has no defined outputs");
        }

        builder.WithOutGroups(elements);
    }

    private static void MapParts(string[] lines, ref int index, ChipDataBuilder builder)
    {
        if (LoopForwardTo(lines, ref index, l => l.StartsWith("PARTS:")))
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
                MapPart(partInfo, builder.AddPart());
            }
        }
    }

    private static void MapPart(string partInfo, ChipPartDataBuilder<ChipDataBuilder> builder)
    {
        var splitPartInfo = partInfo.Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

        builder.WithName(splitPartInfo[0]);

        var links = splitPartInfo.Skip(1).ToArray();

        foreach (var link in links)
        {
            var splitLink = link.Split('=');
            builder.AddLink(splitLink[0], splitLink[1]);
        }

        builder.Build();
    }

    private static bool LoopForwardTo(string[] lines, ref int index, Func<string, bool> selector)
    {
        while (index < lines.Length && !selector(lines[index])) index++;

        return index < lines.Length;
    }

    private static bool GrabArray(string[] lines, ref int index, out string[] elements)
    {
        string fullString = "";
        while (index < lines.Length && !lines[index].Contains(';'))
        {
            fullString += " " + lines[index];    
            index++;
        }

        if (index >= lines.Length)
        {
            elements = Array.Empty<string>();
            return false;
        }

        fullString += " " + lines[index];
        
        var split = fullString.Split(new[] { ' ', ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        elements = split.Skip(1).ToArray();

        return true;
    }

    private static string[] SanitizeInput(string[] input) =>
        input
            .Select(StripCurlyBrackets)
            .StripComments()
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrWhiteSpace(l))
            .ToArray();

    private enum CommentToStrip { None, Inline, Block }
    private static IEnumerable<string> StripComments(this IEnumerable<string> lines)
    {
        var asArray = lines.ToArray();

        for (int n = 0; n < asArray.Length; n++)
        {
            int inlineStart, blockStart;
            do
            {
                inlineStart = asArray[n].IndexOf("//", StringComparison.Ordinal);
                blockStart = asArray[n].IndexOf("/*", StringComparison.Ordinal);

                CommentToStrip commentToStrip = (inlineStart, blockStart) switch
                {
                    (< 0, < 0) => CommentToStrip.None,
                    (< 0, >= 0) => CommentToStrip.Block,
                    (>= 0, < 0) => CommentToStrip.Inline,
                    (>= 0, >= 0) => (inlineStart < blockStart) ? CommentToStrip.Inline : CommentToStrip.Block
                };

                if (commentToStrip == CommentToStrip.Block) 
                    RemoveBlock(asArray, n, blockStart, n);

                if (commentToStrip == CommentToStrip.Inline)
                    asArray[n] = asArray[n].Substring(0, inlineStart);

            } while (inlineStart > -1 || blockStart > -1);
        }

        return asArray;
    }

    private static void RemoveBlock(string[] asArray, int blockStartLine, int blockStartIndex, int blockInitial)
    {
        var subString = asArray[blockStartLine].Substring(blockStartIndex);
        var blockEndIndex = subString.IndexOf("*/", StringComparison.Ordinal);

        var beforeBlock =  asArray[blockStartLine].Substring(0, blockStartIndex);
        if (blockEndIndex == -1)
        {
            asArray[blockStartLine] = "";
            asArray[blockInitial] += beforeBlock;
            RemoveBlock(asArray, blockStartLine + 1, 0, blockInitial);
        }
        else
        {
            var afterBlock = asArray[blockStartLine].Substring(blockStartIndex + blockEndIndex + 2);
            
            asArray[blockStartLine] = "";
            asArray[blockInitial] += beforeBlock + afterBlock;
        }
    }

    private static string StripCurlyBrackets(string line) =>
        line
            .Replace("{", "")
            .Replace("}", "");

}