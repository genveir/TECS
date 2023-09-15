using System;
using System.Collections.Generic;
using System.Linq;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Chips;
using TECS.HDLSimulator.Chips.Factory.Builtins;
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

        _blueprints.Add(new("Nand"), NandBuiltIn.GetBlueprint());
        _blueprints.Add(new("Pass"), PassBuiltIn.GetBlueprint());
        _blueprints.Add(new("Bit"), BitBuiltin.GetBlueprint());
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
}