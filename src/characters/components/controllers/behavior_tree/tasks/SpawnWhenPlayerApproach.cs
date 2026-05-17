namespace Game.Component;

using Godot;
using Godot.Collections;
using Game.Entity;

public partial class SpawnWhenPlayerApproach : BehaviorNode
{
    [Export] public float spawnDistance;
    private bool hasSpawned = false;
    private bool hasStarted = false;
    private bool animStarted = false;
    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        // Already finished
        if (hasSpawned)
            return BehaviorNode.Status.SUCCESS;

        var parent = context["parent"].As<EnemyBase>();
        var target = context["Player"].As<Node2D>();

        float dist = parent.GlobalPosition.DistanceTo(target.GlobalPosition);

        // Error condition
        var animHandler = parent.GetComponent<AnimationHandler>();
        if (animHandler == null)
        {
            GD.PushError("No animation handler.");
            return BehaviorNode.Status.ERROR;
        }

        // Spawn animation is finished
        if (!animHandler.IsCurrentlyPlaying() && animStarted)
        {
            hasSpawned = true;

            parent.EmitSignal(EnemyBase.SignalName.FinishedSpawning);

            return BehaviorNode.Status.SUCCESS;
        }

        // Starts audio effect
        if (!hasStarted && dist <= spawnDistance)
        {
            hasStarted = true;

            var move = parent.GetComponent<Move>();
            if (move != null)
            {
                move.canMove = false;
                move.movementOverride = true;
            }

            parent.EmitSignal(EnemyBase.SignalName.HasSpawned);
            // Shake camera (half)
            var camera = (target as Player).GetComponent<OffsetCamera>();
            camera.add_trauma(0.25f);
        }

        var time = parent.spawn_audioplayer.GetPlaybackPosition() +
            AudioServer.GetTimeSinceLastMix() - AudioServer.GetOutputLatency();

        if (hasStarted && time >= 2.7)
        {
            animHandler.PlayAnimation("spawn", Vector2.Right);
            animHandler.canAdvance = false;

            animStarted = true;

            // Shake camera full
            var camera = (target as Player).GetComponent<OffsetCamera>();
            camera.add_trauma(0.25f);
        }

        return BehaviorNode.Status.RUNNING;
    }
}
