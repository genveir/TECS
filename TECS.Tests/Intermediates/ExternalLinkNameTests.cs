using System;
using FluentAssertions;
using NUnit.Framework;
using TECS.DataIntermediates.Chip.Names;
using TECS.FileAccess.Mappers;

namespace TECS.Tests.Intermediates;

public class LinkNameTests
{
    [TestCase("a", true)]
    [TestCase("aap", true)]
    [TestCase("in", true)]
    [TestCase("a5", true)]
    [TestCase("5a", false)]
    [TestCase("5", false)]
    [TestCase("true", true)]
    [TestCase("false", true)]
    [TestCase("a[16]", true)]
    [TestCase("a[16", false)]
    [TestCase("[16]", false)]
    [TestCase("a[128]", true)]
    [TestCase("a128]", false)]
    [TestCase("out", true)]
    [TestCase("a128", true)]
    public void ExternalLinkNameIsValid(string name, bool isValid)
    {
        if (!isValid)
            Assert.Throws<ArgumentException>(() => _ = new ExternalLinkName(name));
        else
            Assert.DoesNotThrow(() => new ExternalLinkName(name));
    }
    
    [TestCase("a", true)]
    [TestCase("aap", true)]
    [TestCase("in", true)]
    [TestCase("a5", true)]
    [TestCase("5a", false)]
    [TestCase("5", false)]
    [TestCase("true", false)]
    [TestCase("false", false)]
    [TestCase("a[16]", true)]
    [TestCase("a[16", false)]
    [TestCase("[16]", false)]
    [TestCase("a[128]", true)]
    [TestCase("a128]", false)]
    [TestCase("out", true)]
    [TestCase("a128", true)]
    public void InternalLinkNameIsValid(string name, bool isValid)
    {
        if (!isValid)
            Assert.Throws<ArgumentException>(() => _ = new InternalLinkName(name));
        else
            Assert.DoesNotThrow(() => new InternalLinkName(name));
    } 
}