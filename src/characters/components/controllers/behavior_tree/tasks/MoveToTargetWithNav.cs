namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

public partial class MoveToTargetWithNav : MoveToTarget
{
    private Vector2 target_position;
    private NavigationAgent2D nav = null;
    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (nav == null) return;
        target_position = nav.GetNextPathPosition();
    }

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

        CharacterBase parent = context["parent"].As<CharacterBase>();

        nav = parent.GetComponent<NavigationAgent2D>();
        if (nav == null)
        {
            GD.PushError("No navigation node");
            return BehaviorNode.Status.ERROR;
        }

        nav.TargetPosition = (target as Node2D).GlobalPosition;

        Controller controller = parent.GetComponent<Controller>();


        if (has_reached_target(
            (target as Node2D).Position, parent.GlobalPosition))
        {
            controller.moveInput = Vector2.Zero;
            counter = 0.0f;
            return BehaviorNode.Status.SUCCESS;
        }

        controller.moveInput = target_position - parent.GlobalPosition;

        counter += context["delta"].As<float>();
        if (counter >= timeout)
        {
            counter = 0.0f;
            return BehaviorNode.Status.SUCCESS;
        }

        return BehaviorNode.Status.RUNNING;
    }
}
