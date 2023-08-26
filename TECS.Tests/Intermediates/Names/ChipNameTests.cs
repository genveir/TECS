using System;
using NUnit.Framework;
using TECS.DataIntermediates.Names;

namespace TECS.Tests.Intermediates.Names;

public class ChipNameTests
{
    [TestCase("a", true)]
    [TestCase("aap", true)]
    [TestCase("in", true)]
    [TestCase("a5", true)]
    [TestCase("5a", false)]
    [TestCase("5", false)]
    [TestCase("true", true)]
    [TestCase("false", true)]
    [TestCase("a[16]", false)]
    [TestCase("a[16", false)]
    [TestCase("[16]", false)]
    [TestCase("a[128]", false)]
    [TestCase("a128]", false)]
    [TestCase("out", true)]
    [TestCase("a128", true)]
    public void ChipNameIsValid(string name, bool isValid)
    {
        if (!isValid)
            Assert.Throws<ArgumentException>(() => _ = new ChipName(name));
        else
            Assert.DoesNotThrow(() => _ = new ChipName(name));
    }
}