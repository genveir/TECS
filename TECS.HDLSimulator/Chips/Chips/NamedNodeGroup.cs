using System.Collections.Generic;
using TECS.HDLSimulator.Chips.NandTree;

namespace TECS.HDLSimulator.Chips.Chips;

public class NamedNodeGroup
{
    private string Name { get; }
    
    public INandTreeElement[] Nodes { get; }

    public NamedNodeGroup(string name, int bitSize)
    {
        Name = name;
        Nodes = new INandTreeElement[bitSize];

        _value = new bool[Nodes.Length];
        
        for (int n = 0; n < Nodes.Length; n++) Nodes[n] = new NandPinNode();
    }

    private bool[] _value;
    public bool[] Value
    {
        get
        {
            for (int n = 0; n < Nodes.Length; n++)
            {
                _value[n] = Nodes[n].Value;
            }

            return _value;
        }
    }

    public NamedNodeGroup Clone(long cloneId)
    {
        var newGroup = new NamedNodeGroup(Name, Nodes.Length);

        for (int n = 0; n < Nodes.Length; n++)
            newGroup.Nodes[n] = Nodes[n].Clone(cloneId);

        return newGroup;
    }

    public void Fuse(long fuseId)
    {
        for (int n = 0; n < Nodes.Length; n++)
            Nodes[n] = Nodes[n].Fuse(fuseId);
    }

    private long _validatedInRun = -1;
    public void Validate(List<ValidationError> errors, long validationRun)
    {
        if (validationRun == _validatedInRun)
        {
            errors.Add(new($"{this} is hit multiple times in validation run"));
            return;
        }

        _validatedInRun = validationRun;
        
        foreach (var node in Nodes)
        {
            node.Validate(errors, new(), validationRun);
        }
    }

    public void SetAsInputForValidation(List<ValidationError> errors, long validationRun)
    {
        foreach (var node in Nodes)
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

    public override string ToString()
    {
        return $"NamedPinGroup {Name}[{Nodes.Length}]";
    }
}