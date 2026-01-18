using Godot;
using System;

public partial class FlowControlNode : BehaviorNode
{
    public Godot.Collections.Array<BehaviorNode> nodes = 
        new Godot.Collections.Array<BehaviorNode>();

    protected int m_curr_node = 0;

    public override void _Ready()
    {
        base._Ready();

        Godot.Collections.Array<Node> children = GetChildren();

        foreach (Node child in children)
        {
            if (child is BehaviorNode)
            {
                nodes.Add((BehaviorNode)child);
            }
        }
    }

    protected void reset_state()
    {
        m_curr_node = 0;
    }
}
