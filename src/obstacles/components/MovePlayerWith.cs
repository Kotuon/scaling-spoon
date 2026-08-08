namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

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
        if (!enabled || playerRef == null)
            return;

        playerRef.SetCollisionMaskValue(6, false);

        var amount = (
            (parent.Position - lastPosition) * parent.GlobalScale
        ).Rotated(parent.GlobalRotation);

        playerRef.GlobalPosition += amount;
    }

    protected override void ResolveCollisionEnter(Node node)
    {
        if (!enabled || node is not Player)
            return;

        playerRef = node as Player;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (!enabled || node is not Player)
            return;

        playerRef.SetCollisionMaskValue(6, true);
        playerRef = null;
    }
}
