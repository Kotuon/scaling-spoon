namespace Game.Component;

using Game.Entity;
using Godot.Collections;

public partial class BehaviorTree : BehaviorNode
{
    BehaviorNode root;

    private Dictionary m_context = new Dictionary();

    private bool blocked = false;

    public override void _Ready()
    {
        base._Ready();

        root = (BehaviorNode)GetChild(0);

        m_context.Add("parent", GetParent() as CharacterBase);
        (GetParent() as EnemyBase).StartStun += () =>
        { blocked = true; };

        (GetParent() as EnemyBase).EndStun += () =>
        { blocked = false; };
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        m_context["delta"] = delta;
        root.evaluate(m_context);
    }
}
