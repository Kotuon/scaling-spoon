using Godot;
using Godot.Collections;
using System;

public partial class Selector : FlowControlNode
{
    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        if (nodes.Count == 0)
        {
            reset_state();
            return BehaviorNode.Status.SUCCESS;
        }

        BehaviorNode bt_node = nodes[m_curr_node];
        BehaviorNode.Status result = bt_node.evaluate(context);

        if (result == BehaviorNode.Status.ERROR)
        {
            if (m_curr_node == nodes.Count - 1)
            {
                reset_state();
                return BehaviorNode.Status.ERROR;
            }

            m_curr_node += 1;
        }
        else if (result == BehaviorNode.Status.SUCCESS)
        {
            reset_state();
            return BehaviorNode.Status.SUCCESS;
        }

        return BehaviorNode.Status.RUNNING;
    }
}
