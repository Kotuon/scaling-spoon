using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class Sequence : BehaviorNode
{
    private Godot.Collections.Array<BehaviorNode> nodes;

    private int m_curr_node = 0;

    public override void _Ready()
    {
        base._Ready();

        Array<Node> children = GetChildren();

        foreach (Node child in children)
        {
            if (child is BehaviorNode)
            {
                nodes.Add((BehaviorNode)child);
            }
        }
    }


    private void reset_state()
    {
        m_curr_node = 0;
    }

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
            reset_state();
            return BehaviorNode.Status.ERROR;
        }
        else if (result == BehaviorNode.Status.SUCCESS)
        {
            if (m_curr_node == nodes.Count - 1)
            {
                reset_state();
                return BehaviorNode.Status.SUCCESS;
            }

            m_curr_node += 1;
        }

        return BehaviorNode.Status.RUNNING;
    }


}
