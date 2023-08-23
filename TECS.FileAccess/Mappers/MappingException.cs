using System;

namespace TECS.FileAccess.Mappers;

internal class MappingException : Exception
{
    public MappingException(string message) : base(message)
    {
        
    }
}