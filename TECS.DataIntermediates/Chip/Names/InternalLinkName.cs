using System;

namespace TECS.DataIntermediates.Chip.Names;

public class InternalLinkName : LinkName
{
    public InternalLinkName(string value) : base(value)
    {
        if (value =="true")
            throw new ArgumentException("internal pin can not be true");
        if (value =="false")
            throw new ArgumentException("internal pin can not be false");
    }
}