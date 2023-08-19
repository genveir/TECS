using System;
using System.Collections.Generic;

namespace TECS.HDLSimulator.Chips;

internal class Pin
{
    private readonly int _bitSize;
    private readonly bool[] _value;

    private readonly List<INotifyable> _notifyables;

    internal Pin(int bitSize)
    {
        _bitSize = bitSize;
        _value = new bool[bitSize];

        _notifyables = new();
    }

    public void Connect(INotifyable notifyable)
    {
        if (!_notifyables.Contains(notifyable))
            _notifyables.Add(notifyable);  
    }

    public bool[] Value
    {
        get => _value;
        set
        {
            var copyLength = Math.Min(value.Length, _bitSize);

            Array.Copy(value, value.Length - copyLength, _value, _bitSize - copyLength, copyLength);

            foreach (var notifyable in _notifyables)
            {
                notifyable.Notify();
            }
        }
    }
}