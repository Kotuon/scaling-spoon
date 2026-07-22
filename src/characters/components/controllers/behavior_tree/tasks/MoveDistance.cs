namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

public partial class MoveDistance : BehaviorNode
{
    [Export]
    protected Vector2 offset = Vector2.Zero;

    [Export]
    protected float reach_distance = 10.0f;

    private Vector2 target = Vector2.Zero;
    private bool started = false;

    public override void _Ready() { }

    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        CharacterBase parent = context["parent"].As<CharacterBase>();
        if (!started)
        {
            started = true;

            target = parent.GlobalPosition + offset / parent.GlobalScale;
            GD.Print(parent.GlobalScale);
        }

        Controller controller = parent.GetComponent<Controller>();
        if (HasReachedTarget(parent.GlobalPosition, target))
        {
            started = false;
            controller.moveInput = Vector2.Zero;
            return BehaviorNode.Status.SUCCESS;
        }

        controller.moveInput = target - parent.GlobalPosition;

        return BehaviorNode.Status.RUNNING;
    }

    protected bool HasReachedTarget(Vector2 v1, Vector2 v2)
    {
        float distance = v1.DistanceTo(v2);
        return distance < reach_distance;
    }
}
