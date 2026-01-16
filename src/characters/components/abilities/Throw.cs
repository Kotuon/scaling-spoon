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
    
    public override void Trigger()
    {
        base.Trigger();

        parent.attributes["canMove"] = false;

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

        bool spriteFlip = animHandler.sprite.FlipH;

        var animName = animHandler.GetCurrentAnimation();

        if (spriteFlip)
        {
            startPos.X = -startPos.X;
        }

        var dir = (mouseRef.mouseDir - startPos).Normalized();
        // dir = !dir.IsEqualApprox(Vector2.Zero) ? 
        //     (startPos).Normalized() : dir;

        inst.Position = startPos + parent.Position;
        inst.Rotation = dir.Angle();
        inst.launchDir = dir;
    }
}
