using Godot;
using Godot.Collections;
using System;

public partial class FindTarget : BehaviorNode
{
    [Export] public StringName target_name = "Player";

    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        Node target_node = GetTree().Root.GetChild(0).FindChild(target_name);
        if (target_node == null)
            return BehaviorNode.Status.ERROR;

        context[target_name] = target_node;

        return BehaviorNode.Status.SUCCESS;
    }
}
