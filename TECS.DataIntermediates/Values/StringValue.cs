namespace TECS.DataIntermediates.Values;

public class StringValue : IStringFormattableValue
{
    public string Value { get; }

    public StringValue(string value) => Value = value;

    public string FormatForOutput() => Value;
}