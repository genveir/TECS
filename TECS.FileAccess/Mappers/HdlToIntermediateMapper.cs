using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Chip.Names;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public static class HdlToIntermediateMapper
{
    public static ChipData Map(HdlFile file)
    {
        var lines = file.GetContents();

        lines = SanitizeInput(lines);
        var index = 0;

        var chipName = MapChipName(lines, ref index);
        var inGroups = MapIn(lines, ref index);
        var outGroups = MapOut(lines, ref index);
        var parts = MapParts(lines, ref index);

        return new(
            chipName,
            inGroups,
            outGroups,
            parts);
    }

    private static ChipName MapChipName(string[] lines, ref int index)
    {
        if (LoopForwardTo(lines, ref index, l => l.StartsWith("CHIP")))
        {
            var split = lines[index].Split(' ');
            return new(split[1]);
        }

        throw new MappingException("HDL file has no defined chip");
    }

    private static IEnumerable<NamedNodeGroupName> MapIn(string[] lines, ref int index)
    {
        if (LoopForwardTo(lines, ref index, l => l.StartsWith("IN")) &&
            GrabArray(lines, ref index, out string[] elements))
        {
            return elements.Select(e => new NamedNodeGroupName(e));
        }

        throw new MappingException("HDL file has no defined inputs");
    }

    private static IEnumerable<NamedNodeGroupName> MapOut(string[] lines, ref int index)
    {
        if (LoopForwardTo(lines, ref index, l => l.StartsWith("OUT")) &&
            GrabArray(lines, ref index, out string[] elements))
        {
            return elements.Select(e => new NamedNodeGroupName(e));
        }

        throw new MappingException("HDL file has no defined outputs");
    }

    private static IEnumerable<ChipPartData> MapParts(string[] lines, ref int index)
    {
        List<ChipPartData> chipParts = new();
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
                chipParts.Add(MapPart(partInfo));
            }
        }

        // we are okay with empty parts, it's the start state of every project HDL
        return chipParts;
    }

    private static ChipPartData MapPart(string partInfo)
    {
        var splitPartInfo = partInfo.Split(new[] { '(', ',', ')' }, StringSplitOptions.RemoveEmptyEntries);

        ChipName name = new(splitPartInfo[0]);

        var links =  splitPartInfo.Skip(1).Select(MapLink);

        return new(name, links);
    }

    private static LinkData MapLink(string linkInfo)
    {
        var splitInfo = linkInfo.Split('=');

        var internalName = new InternalLinkName(splitInfo[0]);
        var externalName = new ExternalLinkName(splitInfo[1]);

        return new(internalName, externalName);
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