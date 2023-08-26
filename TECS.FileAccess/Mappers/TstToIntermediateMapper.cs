using System.Collections.Generic;
using TECS.DataIntermediates.Builders;
using TECS.DataIntermediates.Test;
using TECS.FileAccess.FileAccessors;

namespace TECS.FileAccess.Mappers;

public class TstToIntermediateMapper
{
    private static readonly Dictionary<TestFile, TestData> Mapped = new();

    public static TestData Map(HdlFolder hdlFolder, TestFile testFile)
    {
        if (!Mapped.TryGetValue(testFile, out var testData))
        {
            var builder = new TestDataBuilder();

            testData = builder.Build();
            Mapped[testFile] = testData;
        }

        return testData;
    }
}