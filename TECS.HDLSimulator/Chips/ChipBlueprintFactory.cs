using System.Collections.Generic;
using System.Linq;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips;

public class ChipBlueprintFactory
{
    private readonly IEnumerable<ChipDescription> _descriptions;
    private readonly Dictionary<string, ChipBlueprint> _blueprints = new();

    public ChipBlueprintFactory(IEnumerable<ChipDescription> descriptions)
    {
        _descriptions = descriptions;
    }

    public void BuildBlueprints()
    {
        
    }

    public void BuildBlueprint(ChipDescription description)
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

        return new ChipBlueprint("Nand", inputs, "out", nandNode);
    }
}