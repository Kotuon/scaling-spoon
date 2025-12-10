using Game.Component;
using Godot;
using Godot.Collections;
using System;

public partial class BehaviorNode : Node
{
    public enum Status
    {
        RUNNING,
        SUCCESS,
        ERROR
    }

    virtual public Status evaluate(Dictionary context)
    {
        GD.PushError("Not implemented evaluate in behavior tree, " + Name);
        return Status.ERROR;
    }
}

public partial class DecoratorNode : BehaviorNode
{
    protected BehaviorNode m_child;

    public override void _Ready()
    {
        base._Ready();

        m_child = (BehaviorNode)GetChild(0);
    }
}

public partial class FlowControlNode : BehaviorNode
{
    public Godot.Collections.Array<BehaviorNode> nodes = 
        new Array<BehaviorNode>();

    protected int m_curr_node = 0;

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

    protected void reset_state()
    {
        m_curr_node = 0;
    }
}
