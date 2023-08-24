using System;
using System.Collections.Generic;
using NUnit.Framework;
using TECS.DataIntermediates.Chip;
using TECS.DataIntermediates.Chip.Names;

namespace TECS.Tests.Intermediates;

public class ChipDataTests
{
    [Test]
    public void CanCreateChipData()
    {
        var chipName = ValidChipName;

        var internalPins = new List<NamedNodeGroupName>() { ValidNodeGroupName, ValidNodeGroupName };
        var externalPins = new List<NamedNodeGroupName>() { ValidNodeGroupName, ValidNodeGroupName };

        var parts = Array.Empty<ChipPartData>();
        
        _ = new ChipData(chipName, internalPins, externalPins, parts);
    }

    [Test]
    public void CannotCreateChipDataWithDoubleInternalGroupNames()
    {
        var chipName = ValidChipName;

        var nodeGroupName = ValidNodeGroupName;

        var interalPins = new List<NamedNodeGroupName> { nodeGroupName, nodeGroupName };
        var externalPins = new List<NamedNodeGroupName> { ValidNodeGroupName, ValidNodeGroupName };
        
        var parts = Array.Empty<ChipPartData>();

        Assert.Throws<ArgumentException>(() => 
            _ = new ChipData(chipName, interalPins, externalPins, parts));
    }
    
    [Test]
    public void CannotCreateChipDataWithDoubleExternalGroupNames()
    {
        var chipName = ValidChipName;

        var nodeGroupName = ValidNodeGroupName;

        var interalPins = new List<NamedNodeGroupName> { ValidNodeGroupName, ValidNodeGroupName };
        var externalPins = new List<NamedNodeGroupName> { nodeGroupName, nodeGroupName };
        
        var parts = Array.Empty<ChipPartData>();

        Assert.Throws<ArgumentException>(() => 
            _ = new ChipData(chipName, interalPins, externalPins, parts));
    }
    
    [Test]
    public void CannotCreateChipDataWithGroupNameInBothInAndOutputs()
    {
        var chipName = ValidChipName;

        var nodeGroupName = ValidNodeGroupName;

        var interalPins = new List<NamedNodeGroupName> { nodeGroupName, ValidNodeGroupName };
        var externalPins = new List<NamedNodeGroupName> { nodeGroupName, ValidNodeGroupName };
        
        var parts = Array.Empty<ChipPartData>();

        Assert.Throws<ArgumentException>(() => 
            _ = new ChipData(chipName, interalPins, externalPins, parts));
    }

    [Test]
    public void CannotCreatePartDataWithoutLinks()
    {
        Assert.Throws<ArgumentException>(() =>
            _ = new ChipPartData(ValidChipName, Array.Empty<LinkData>()));
    }

    private int _nameCounter;
    
    private ChipName ValidChipName => new("test" + _nameCounter++);

    private NamedNodeGroupName ValidNodeGroupName => new("test" + _nameCounter++);
}