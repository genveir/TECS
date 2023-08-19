using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips;

public class ChipFactory
{
    private readonly IEnumerable<ChipSummary> _summaries;
    
    public ChipFactory(IEnumerable<ChipSummary> summaries)
    {
        _summaries = summaries;
    }
}