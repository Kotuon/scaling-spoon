namespace Game.Component;

using Godot;
using Godot.Collections;
using Game.Entity;

public partial class SpawnWhenPlayerApproach : BehaviorNode
{
    [Export] public float spawnDistance;
    private bool hasSpawned = false;
    private bool hasStarted = false;
    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        if (hasSpawned)
            return BehaviorNode.Status.SUCCESS;

        var parent = context["parent"].As<CharacterBase>();
        var target = context["Player"].As<Node2D>();

        float dist = parent.GlobalPosition.DistanceTo(target.GlobalPosition);
        // GD.Print(dist);

        var animHandler = parent.GetComponent<AnimationHandler>();
        if (animHandler == null)
        {
            GD.PushError("No animation handler.");
            return BehaviorNode.Status.ERROR;
        }

        if (!animHandler.IsCurrentlyPlaying() && hasStarted)
        {
            hasSpawned = true;

            return BehaviorNode.Status.SUCCESS;
        }

        if (!hasStarted && dist <= spawnDistance)
        {
            hasStarted = true;

            var move = parent.GetComponent<Move>();
            if (move != null)
            {
                move.canMove = false;
                move.movementOverride = true;
            }

            animHandler.PlayAnimation("spawn", Vector2.Right);
            animHandler.canAdvance = false;
        }

        return BehaviorNode.Status.RUNNING;
    }
}
