using System;
using System.Linq;

namespace TECS.FileAccess.Mappers;

public static class StringArrayNavigator
{
    public static bool GetNext(string[] lines, ref int index)
    {
        index++;

        return index < lines.Length;
    }
    
    public static bool LoopForwardTo(string[] lines, ref int index, Func<string, bool> selector)
    {
        while (index < lines.Length && !selector(lines[index])) index++;

        return index < lines.Length;
    }

    public static bool GrabArray(string[] lines, ref int index, out string[] elements)
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
}