using System;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Names;

namespace TECS.Tests.Intermediates.Chip;

public class ChipDataTests
{
    [Test]
    public void CanCreateChipData()
    {
        var chipName = ValidChipName;

        var internalPins = new[] { ValidNodeGroupData, ValidNodeGroupData };
        var externalPins = new[] { ValidNodeGroupData, ValidNodeGroupData };

        var parts = Array.Empty<ChipPartData>();
        
        _ = new ChipData(chipName, internalPins, externalPins, parts);
    }

    [Test]
    public void CannotCreateChipDataWithDoubleInternalGroupNames()
    {
        var chipName = ValidChipName;

        var nodeGroupName = ValidNodeGroupData;

        var internalPins = new[] { nodeGroupName, nodeGroupName };
        var externalPins = new[] { ValidNodeGroupData, ValidNodeGroupData };
        
        var parts = Array.Empty<ChipPartData>();

        Assert.Throws<ArgumentException>(() => 
            _ = new ChipData(chipName, internalPins, externalPins, parts));
    }
    
    [Test]
    public void CannotCreateChipDataWithDoubleExternalGroupNames()
    {
        var chipName = ValidChipName;

        var nodeGroupName = ValidNodeGroupData;

        var internalPins = new[] { ValidNodeGroupData, ValidNodeGroupData };
        var externalPins = new[] { nodeGroupName, nodeGroupName };
        
        var parts = Array.Empty<ChipPartData>();

        Assert.Throws<ArgumentException>(() => 
            _ = new ChipData(chipName, internalPins, externalPins, parts));
    }
    
    [Test]
    public void CannotCreateChipDataWithGroupNameInBothInAndOutputs()
    {
        var chipName = ValidChipName;

        var nodeGroupName = ValidNodeGroupData;

        var internalPins = new[] { nodeGroupName, ValidNodeGroupData };
        var externalPins = new[] { nodeGroupName, ValidNodeGroupData };
        
        var parts = Array.Empty<ChipPartData>();

        Assert.Throws<ArgumentException>(() => 
            _ = new ChipData(chipName, internalPins, externalPins, parts));
    }

    [Test]
    public void CannotCreatePartDataWithoutLinks()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new ChipPartData(ValidChipName, Array.Empty<LinkData>()));
    }

    private int _nameCounter;
    
    private ChipName ValidChipName => new("test" + _nameCounter++);

    private NamedNodeGroupData ValidNodeGroupData => new(new("test" + _nameCounter++), new(1));
}