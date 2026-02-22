namespace Game.Component;

using Game.Entity;
using Godot;
using System;

public partial class MovePlayerWith : ObstacleComponent
{
    private Player playerRef = null;
    public override void _Ready()
    {
        base._Ready();

        parent.moved += ResolveMove;
    }

    private void ResolveMove(Vector2 lastPosition)
    {
        if (!enabled || playerRef == null) return;

        playerRef.GlobalPosition += (parent.GlobalPosition - lastPosition);
    }

    protected override void ResolveCollisionEnter(Node node)
    {
        if (!enabled || node is not Player) return;

        playerRef = node as Player;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (!enabled || node is not Player) return;

        playerRef = null;
    }
}
