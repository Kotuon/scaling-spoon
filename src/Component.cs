namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class Component : Node2D
{
    public CharacterBase parent;
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
