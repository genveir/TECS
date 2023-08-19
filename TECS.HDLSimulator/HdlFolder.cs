using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TECS.HDLSimulator;

internal class HdlFolder
{
    private readonly string _path;
    private IEnumerable<HdlFile>? _files;

    public HdlFolder(string path)
    {
        _path = path;
    }

    private IEnumerable<HdlFile> FindFiles()
    {
        var fileNames = Directory.GetFiles(_path, "*.hdl");

        return fileNames.Select(fn => new HdlFile(Path.GetFileNameWithoutExtension(fn), fn));
    }

    public IEnumerable<HdlFile> GetFiles()
    {
        _files = _files ??= FindFiles();

        return _files;
    }
}