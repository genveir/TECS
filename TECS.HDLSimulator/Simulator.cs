using System.IO;
using System.Linq;

namespace TECS.HDLSimulator;

public class Simulator
{
    public Simulator(string dataFolder)
    {
        var folder = new HdlFolder(Path.Combine(dataFolder, "HDL"));

        var files = folder.GetFiles();

        var contents = files.Select(f => f.GetContents());

        var summaries = contents.Select(HdlParser.ParseSummary).ToArray();

        ;
    }
}