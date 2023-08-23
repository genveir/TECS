using System.Collections.Generic;
using System.IO;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess;

public class HdlFolder
{
    public readonly HdlFile[] HdlFiles;
    public readonly TestFile[] TestFiles;
    public readonly ComparisonFile[] ComparisonFiles;

    public HdlFolder(string path)
    {
        var filePaths = Directory.GetFiles(path);

        var hdlFiles = new List<HdlFile>();
        var testFiles = new List<TestFile>();
        var comparisonFiles = new List<ComparisonFile>();
        
        foreach (var filePath in filePaths)
        {
            var extension = Path.GetExtension(filePath);

            switch (extension)
            {
                case ".hdl": 
                    hdlFiles.Add(new(filePath));
                    break;
                case ".tst": 
                    testFiles.Add(new(filePath));
                    break;
                case ".cmp":
                    comparisonFiles.Add(new(filePath));
                    break;
            }
        }

        HdlFiles = hdlFiles.ToArray();
        TestFiles = testFiles.ToArray();
        ComparisonFiles = comparisonFiles.ToArray();
    }
}