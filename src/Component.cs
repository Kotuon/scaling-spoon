namespace Game.Component;

using Godot;
using System;

public partial class Component : Node2D
{
    private bool _enabled = true;
    
    [Export]
    public bool Enabled
    {
        set => SetEnabled(value);
        get => _enabled;
    }

    protected virtual void _EnabledPostProcess(){}

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
        _EnabledPostProcess();
    }
}
