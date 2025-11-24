namespace Game.Component;

using Game.Entity;
using Godot;
using System;

public partial class OffsetCamera : Camera2D
{    
    private Vector2 targetOffset = Vector2.Zero;
    private float targetSpeed = 0.125f;
    
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

    private bool useOffset = false;

    public void TriggerOffset(Vector2 target, float speed = 0.125f)
    {
        targetOffset = target;
        targetSpeed = speed;
    }

    public void CancelOffset()
    {
        targetOffset = Vector2.Zero;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Update();
    }

    private void Update()
    {
        Offset = Offset.Lerp(targetOffset, targetSpeed);
    }
}
