using System;
using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips;

public interface INandTreeNode
{
    bool[] Value { get; }
}

public class NandPinNode : INandTreeNode
{
    public INandTreeNode? Parent { get; set; }

    private bool[] _value = Array.Empty<bool>();
    public bool[] Value
    {
        get => Parent == null ? _value : Parent.Value;
        set => _value = value;
    }
}

public class NandNode : INandTreeNode
{
    public NandPinNode A = new();
    public NandPinNode B = new();

    public bool[] Value => new[] { !(A.Value[0] && B.Value[0]) };
}