using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprintFactory
{
    private readonly IEnumerable<ChipSummary> _summaries;
    private readonly Dictionary<string, ChipBlueprint> _blueprints = new();

    public ChipBlueprintFactory(IEnumerable<ChipSummary> summaries)
    {
        _summaries = summaries;
    }

    public void BuildBlueprints()
    {
        
    }

    private ChipBlueprint NandBlueprint()
    {
        var nandNode = new NandNode();

        nandNode.TryGetInputPins(out var inputNodes);
        
        var inputs = new Dictionary<string, NandPinNode>
        {
            { "a", inputNodes.a },
            { "b", inputNodes.b }
        };

        return new ChipBlueprint(inputs, "out", nandNode);
    }
}