namespace Game.Component;

using Godot;
using System;

public partial class Controller : Component
{
    protected Vector2 _moveInput;
    public virtual Vector2 moveInput
    {
        set
        {
            _moveInput = value.Normalized();
        }
        get
        {
            return _moveInput;
        }
    }
}
