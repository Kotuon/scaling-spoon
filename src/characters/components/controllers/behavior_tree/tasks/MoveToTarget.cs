namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

public partial class MoveToTarget : BehaviorNode
{
    [Export]
    public StringName target_name = "Player";

    [Export]
    public float reach_distance = 400.0f;

    [Export]
    public float timeout = 10.0f;
    protected float counter = 0.0f;

    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        if (!context.ContainsKey(target_name))
        {
            GD.PushError("Target is not in context.");
            return BehaviorNode.Status.ERROR;
        }

        Node target = context[target_name].As<Node>();
        if (target is not Node2D)
        {
            GD.PushError("Target is not node 2d, got %s", target);
            return BehaviorNode.Status.ERROR;
        }

        CharacterBase parent = context["parent"].As<CharacterBase>();

        Controller controller = parent.GetComponent<Controller>();

        Vector2 target_position = (target as Node2D).GlobalPosition;
        if (has_reached_target(target_position, parent.GlobalPosition))
        {
            controller.moveInput = Vector2.Zero;
            return BehaviorNode.Status.SUCCESS;
        }

        controller.moveInput = target_position - parent.GlobalPosition;

        return BehaviorNode.Status.RUNNING;
    }

    protected bool has_reached_target(Vector2 p1, Vector2 p2)
    {
        float distance = p1.DistanceTo(p2);
        return distance < reach_distance;
    }
}
