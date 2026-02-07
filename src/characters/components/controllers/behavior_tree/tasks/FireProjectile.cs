using Godot;
using Godot.Collections;
using System;
using Game.Entity;
using Game.Component;

public partial class FireProjectile : BehaviorNode
{
    [Export] public PackedScene proj;
    [Export] public float damage;
    [Export] public StringName target_name = "Player";

    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        if (!context.ContainsKey(target_name))
        {
            GD.PushError("Target is not in context.");
            return BehaviorNode.Status.ERROR;
        }

        Node target = context[target_name].As<Node>();
        if (target is not Node2D)
        {
            GD.PushError("Target is not node 2d, got %s", target);
            return BehaviorNode.Status.ERROR;
        }

        CharacterBase parent = context["parent"].As<CharacterBase>();

        Vector2 target_position = (target as Node2D).Position;

        SpawnProjectile(parent,
            (target_position - parent.Position).Normalized());

        return BehaviorNode.Status.SUCCESS;
    }

    private void SpawnProjectile(CharacterBase parent, Vector2 dir)
    {
        var inst = (Projectile)proj.Instantiate();
        parent.GetParent().AddChild(inst);

        Vector2 startPos = dir * 80.0f;

        inst.Position = startPos + parent.Position;
        inst.Rotation = dir.Angle();
        inst.launchDir = dir;
        inst.owner = parent;

        inst.damage = damage;
    }
}
