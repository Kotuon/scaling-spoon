using Game.Entity;
using Godot;
using Godot.Collections;
using System;

public partial class BehaviorTree : BehaviorNode
{
    BehaviorNode root;

    private Dictionary m_context = new Dictionary();

    public override void _Ready()
    {
        base._Ready();

        root = (BehaviorNode)GetChild(0);

        m_context.Add("parent", (CharacterBase)GetParent());
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        root.evaluate(m_context);
    }

}
