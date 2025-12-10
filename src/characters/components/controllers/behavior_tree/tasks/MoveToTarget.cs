using Godot;
using Godot.Collections;
using System;
using Game.Entity;
using Game.Component;

public partial class MoveToTarget : BehaviorNode
{
    [Export] public StringName target_name = "Player";
    [Export] public float reach_distance = 200.0f;
    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        if (!context.ContainsKey(target_name))
        {
            GD.PushError("Target is not in context.");
            return BehaviorNode.Status.ERROR;
        }

        Node target = (Node)context[target_name];
        if (target is not Node2D)
        {
            GD.PushError("Target is not node 2d, got %s", target);
            return BehaviorNode.Status.ERROR;
        }

        CharacterBase parent = (CharacterBase)context["parent"];

        Controller controller = parent.GetComponent<Controller>();

        Vector2 target_position = (target as Node2D).Position;
        if (has_reached_target(target_position, parent.Position))
        {
            controller.moveInput = Vector2.Zero;
            return BehaviorNode.Status.SUCCESS;
        }

        controller.moveInput = target_position - parent.Position;

        return BehaviorNode.Status.RUNNING;
    }

    private bool has_reached_target(Vector2 p1, Vector2 p2)
    {
        float distance = p1.DistanceTo(p2);
        return distance < reach_distance;
    }
}
