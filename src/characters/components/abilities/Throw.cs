namespace Game.Component;

using Godot;
using System;

public partial class Throw : Ability
{
    [Export] public PackedScene proj;
    [Export] public float damage;

    [Export(PropertyHint.Range, "10, 800, or_greater, or_less")]
    public float startDistance = 60.0f;

    public Throw() : base("throw")
    {

    }

    public override void _Ready()
    {
        base._Ready();
    }


    public override void Trigger()
    {
        base.Trigger();

        if (!isActive) return;

        move.canMove = false;

        animHandler.PlayAnimation(abilityName + "_init", mouseRef.mouseDir);
        mouseRef.useMouseDirection = true;
    }

    public override void Update(double delta)
    {
        base.Update(delta);

        if (animHandler.GetCurrentAnimation().Find(abilityName + "_init") == -1)
        {
            animHandler.PlayAnimation(
                abilityName + "_update", mouseRef.mouseDir);
        }
    }

    public override void End()
    {
        base.End();

        animHandler.PlayAnimation(abilityName + "_end", mouseRef.mouseDir);
        animHandler.canAdvance = false;
    }

    public void SpawnProjectile()
    {
        var inst = (Projectile)proj.Instantiate();

        parent.GetParent().AddChild(inst);

        Vector2 startPos = mouseRef.mouseDir * startDistance;

        var dir = (startPos - mouseRef.mouseDir).Normalized();

        inst.Position = startPos + parent.Position;
        inst.Rotation = dir.Angle();
        inst.launchDir = dir;
        inst.owner = parent;

        inst.damage = damage;
    }
}
