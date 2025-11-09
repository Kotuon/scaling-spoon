namespace Game.Component;

using Game.Entity.Player;
using Godot;
using System;

public partial class MouseCamera : Camera2D
{
    [Export]
    public float offsetPercent = 0.25f;

    private Player _parent;
    protected Player parent
    {
        private set => _parent = value;

        get
        {
            if (_parent == null)
                _parent = (Player)GetNode<Player>("..");
            return _parent;
        }
    }

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

    private Vector2 targetLoc = Vector2.Zero; 

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!mouseRef.useMouseDirection)
            targetLoc = Vector2.Zero;
        else
        {
            var between = mouseRef.GlobalPosition - parent.GlobalPosition;

            targetLoc = between * offsetPercent;
        }

        Update();
    }

    private void Update()
    {
        Offset = Offset.Lerp(targetLoc, 0.125f);
    }
}
