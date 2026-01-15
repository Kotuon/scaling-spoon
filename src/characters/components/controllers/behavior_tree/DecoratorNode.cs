using Godot;
using System;

public partial class DecoratorNode : BehaviorNode
{
    protected BehaviorNode m_child;

    public override void _Ready()
    {
        base._Ready();

        m_child = (BehaviorNode)GetChild(0);
    }
}
