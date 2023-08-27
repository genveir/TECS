using System.Collections.Generic;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Test;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public class TstToIntermediateMapper
{
    private static readonly Dictionary<TestFile, TestData> Mapped = new();

    public static TestData Map(HdlFolder hdlFolder, TestFile file)
    {
        if (!Mapped.TryGetValue(file, out var testData))
        {
            var lines = file.GetContents();

            lines = StringArraySanitizer.SanitizeInput(lines);
            var index = 0;
            
            var builder = new TestDataBuilder();

            testData = builder.Build();
            Mapped[file] = testData;
        }

        return testData;
    }
}