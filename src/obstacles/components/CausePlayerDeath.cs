namespace Game.Component;

using Game.Entity;
using Godot;
using System;

public partial class CausePlayerDeath : ObstacleComponent
{
    public override void _Ready()
    {
        base._Ready();

        parent.effectPlayer += EffectPlayer;
    }

    protected override void ResolveCollisionEnter(Node node)
    {
        if (!enabled || node is not Player) return;

        PlayerCollider collider =
            (node as Player).GetComponent<PlayerCollider>();

        collider.SetObstacle(parent, true);

        if (collider.ignoreObstacles) return;

        EffectPlayer(node as Player);
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (!enabled || node is not Player) return;

        PlayerCollider collider =
            (node as Player).GetComponent<PlayerCollider>();

        collider.SetObstacle(null, false);
    }

    private void EffectPlayer(Player playerRef)
    {
        playerRef.Dies();
    }
}
