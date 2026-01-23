namespace Game.Component;

using Godot;
using System;

public partial class Throw : Ability
{
    [Export]
    public PackedScene proj;

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

        move.canMove = false;

        animHandler.PlayAnimation("throw_init", mouseRef.mouseDir);
        mouseRef.useMouseDirection = true;
    }

    public override void Update(double delta)
    {
        base.Update(delta);

        if (animHandler.GetCurrentAnimation().Find("throw_init") == -1)
            animHandler.PlayAnimation("throw_update", mouseRef.mouseDir);
    }

    public override void End()
    {
        base.End();

        animHandler.PlayAnimation("throw_end", mouseRef.mouseDir);
        animHandler.canAdvance = false;
    }
    
    public void SpawnProjectile(Vector2 startPos)
    {
        var inst = (Projectile)proj.Instantiate();
        GetNode("../../..").AddChild(inst);

        // startPos = (mouseRef.Position - parent.Position).Normalized();
        startPos = mouseRef.mouseDir * 60.0f;
        
        var dir = (startPos - mouseRef.mouseDir).Normalized();

        inst.Position = startPos + parent.Position;
        inst.Rotation = dir.Angle();
        inst.launchDir = dir;
        inst.owner = parent;
    }
}
