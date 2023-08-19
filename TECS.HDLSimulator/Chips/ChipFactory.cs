using System.Collections.Generic;
using System.Linq;

namespace TECS.HDLSimulator.Chips;

internal class ChipFactory
{
    private readonly ChipSummary[] _summaries;

    private readonly Dictionary<string, Chip> _constructedChips = new();

    internal ChipFactory(ChipSummary[] summaries)
    {
        _summaries = summaries;
    }

    public Chip[] ConstructAll()
    {
        foreach (var summary in _summaries)
        {
            Construct(summary.Name);
        }

        return _constructedChips.Values.ToArray();
    }

    public Chip Construct(string name) => Construct(name, new());

    public Chip Construct(string name, Dictionary<string, Pin> pinsToUse)
    {
        var summary = _summaries.Single(sum => sum.Name == name);

        Dictionary<string, Pin> allPins = new();

        Dictionary<string, Pin> inPins = new();
        foreach (var pinSummary in summary.In)
        {
            if (!pinsToUse.TryGetValue(pinSummary.Name, out var pin))
                pin = new Pin(pinSummary.BitSize);
            
            inPins.Add(pinSummary.Name, pin);
            allPins.Add(pinSummary.Name, pin);
        }

        Dictionary<string, Pin> outPins = new();
        foreach (var pinSummary in summary.Out)
        {
            if (!pinsToUse.TryGetValue(pinSummary.Name, out var pin))
                pin = new Pin(pinSummary.BitSize);

            outPins.Add(summary.Name, pin);
            allPins.Add(summary.Name, pin);
        }

        foreach (var partSummary in summary.Parts)
        {
            
        }

        _constructedChips[summary.Name] = new Chip(summary.Name, inPins, outPins);
        return _constructedChips[name];
    }
}