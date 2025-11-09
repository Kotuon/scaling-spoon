namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class Component : Node2D
{
    public CharacterBase parent;
    private Mouse _mouseRef;
    protected Mouse mouseRef
    {
        private set
        {
            _mouseRef = value;
        }

        get
        {
            if (_mouseRef == null)
                _mouseRef = parent.GetComponent<Mouse>();
            return _mouseRef;
        }
    }
    private bool _enabled = true;

    [Export]
    public bool Enabled
    {
        set => SetEnabled(value);
        get => _enabled;
    }

    protected virtual void _EnabledPostProcess() { }

    public void SetEnabled(bool enabled)
    {
        _enabled = enabled;
        _EnabledPostProcess();
    }
}
