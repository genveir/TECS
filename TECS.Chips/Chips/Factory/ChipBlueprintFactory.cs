using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Factory;

public class ChipBlueprintFactory : IChipBlueprintFactory
{
    private readonly IEnumerable<ChipData> _allChipData;
    private readonly StoredBlueprintFactory _storedBlueprintFactory;

    private readonly Dictionary<ChipName, StoredBlueprint> _blueprints = new();

    public ChipBlueprintFactory(IEnumerable<ChipData> allChipData)
    {
        _allChipData = allChipData;
        _storedBlueprintFactory = new(this);

        _blueprints.Add(new("Nand"), NandByHand());
        _blueprints.Add(new("Pass"), PassByHand());
    }

    public FactoryResult<ChipBlueprint> GetBlueprint(ChipName name)
    {
        if (!_blueprints.TryGetValue(name, out StoredBlueprint? blueprint))
        {
            var chipData = _allChipData.SingleOrDefault(cd => cd.Name.Equals(name));

            if (chipData != null)
            {
                var createStoredResult = _storedBlueprintFactory.CreateBlueprint(chipData);
                if (!createStoredResult.Success)
                    return FactoryResult<ChipBlueprint>.Fail(createStoredResult.Errors);

                blueprint = createStoredResult.Result;
                
                if (blueprint == null)
                    throw new InvalidOperationException("create stored returned null, should not be possible");
                _blueprints.Add(name, blueprint);
            }
            else
            {
                return FactoryResult<ChipBlueprint>.Fail(new List<ValidationError>
                    { new($"factory is missing required chip data for {name}") });
            }
        }

        if (blueprint.ValidationErrors.Any())
            return FactoryResult<ChipBlueprint>.Fail(blueprint.ValidationErrors);

        return
            FactoryResult<ChipBlueprint>.Succeed(blueprint.CopyToBlueprintInstance());
    }

    private static StoredBlueprint NandByHand()
    {
        var aInput = new InputNodeGroup(new(new("a"), new BitSize(1)));
        var bInput = new InputNodeGroup(new(new("b"), new BitSize(1)));

        var nandNode = new NandNode(aInput.Nodes[0], bInput.Nodes[0]);

        var outputGroup = OutputNodeGroup.NandGroup(nandNode);

        var inputs = new Dictionary<NamedNodeGroupName, InputNodeGroup>
        {
            { new("a"), aInput },
            { new("b"), bInput }
        };
        var outputs = new Dictionary<NamedNodeGroupName, OutputNodeGroup>
        {
            { new("out"), outputGroup }
        };

        return new(new("Nand"), inputs, outputs);
    }

    private static StoredBlueprint PassByHand()
    {
        var input = new InputNodeGroup(new(new("in"), new BitSize(1)));
        var outPin = new PinBoard(new("out"), new BitSize(1));
        outPin.Nodes[0].Parent = input.Nodes[0];
        var output = new OutputNodeGroup(outPin);

        var inputs = new Dictionary<NamedNodeGroupName, InputNodeGroup>
        {
            { new("in"), input }
        };
        var outputs = new Dictionary<NamedNodeGroupName, OutputNodeGroup>
        {
            { new("out"), output }
        };

        return new(new("Pass"), inputs, outputs);
    }
}