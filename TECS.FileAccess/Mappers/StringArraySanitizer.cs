using System;
using System.Collections.Generic;
using System.Linq;

namespace TECS.FileAccess.Mappers;

public static class StringArraySanitizer
{
    public static string[] SanitizeInput(string[] input) =>
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