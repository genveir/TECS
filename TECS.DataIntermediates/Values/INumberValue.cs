namespace TECS.DataIntermediates.Values;

public interface INumberValue
{
    BitValue AsBitValue();

    LongValue AsLongValue();
}