using System;

namespace TECS.DataIntermediates.Names;

public class ExternalLinkName : NamedNodeGroupName
{
    public ExternalLinkName(string value) : base(value, false) {}
}