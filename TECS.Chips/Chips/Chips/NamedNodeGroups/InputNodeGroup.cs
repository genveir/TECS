using System;
using System.Collections.Generic;
using TECS.DataIntermediates.Names;
using TECS.HDLSimulator.Chips.Factory;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips.NamedNodeGroups;

public class InputNodeGroup : NamedNodeGroup<InputNodeGroup>
{
    internal override ReadOnlySpan<INandTreeElement> Nodes => Pins;
    
    internal readonly NandPinNode[] Pins;
    
    private InputNodeGroup(NamedNodeGroupName name, NandPinNode[] pins) : base(name)
    {
        Pins = pins;
    }
    
    internal InputNodeGroup(PinBoard pinBoard) : this(pinBoard.Name, pinBoard.Nodes) { }
    
    internal void SetValue(BitValue value)
    {
        if (value.Size.Value == Pins.Length)
        {
            for (int n = 0; n < Pins.Length; n++)
            {
                Pins[n].SetValue(value.Value[n]);
            }
        }
    }

    public void SetAsInputForValidation(List<ValidationError> errors, long validationRun)
    {
        foreach (var node in Pins)
        {
            node.SetAsInputForValidation(errors, validationRun);
        }
    }
    
    public void IsValidatedInRun(List<ValidationError> errors, long validationRun)
    {
        foreach (var node in Nodes)
        {
            if (!node.IsValidatedInRun(validationRun))
                errors.Add(new($"{node} is an unconnected input"));
        }
    }

    internal override InputNodeGroup Clone(long cloneId)
    {
        var newNodes = new NandPinNode[Pins.Length];
        for (int n = 0; n < newNodes.Length; n++)
        {
            newNodes[n] = Pins[n].ClonePin(cloneId);
        }

        return new(Name, newNodes);
    }
}