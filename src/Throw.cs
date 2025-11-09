namespace Game.Component;

using Godot;
using System;

public partial class Throw : Ability
{
    [Export]
    public PackedScene proj;
    
    public override void Trigger()
    {
        base.Trigger();
        
        move._canMove = false;
        
        Godot.Vector2 inputDirection = Input.GetVector("walk_left",
            "walk_right", "walk_up", "walk_down");

        animHandler.PlayAnimation(inputDirection, "throw_init");
        mouseRef.useMouseDirection = true;
    }

    public override void Update()
    {
        base.Update();

        Godot.Vector2 inputDirection = Input.GetVector("walk_left",
            "walk_right", "walk_up", "walk_down");

        if (animHandler.GetCurrentAnimation().Find("throw_init") == -1)
            animHandler.PlayAnimation(inputDirection, "throw_update");
    }

    public override void End()
    {
        base.End();

        Godot.Vector2 inputDirection = Input.GetVector("walk_left",
            "walk_right", "walk_up", "walk_down");
        animHandler.PlayAnimation(inputDirection, "throw_end");

        animHandler.canAdvance = false;
    }
    
    public void SpawnProjectile(Vector2 startPos)
    {
        var inst = (Projectile)proj.Instantiate();
        GetNode("../..").AddChild(inst);

        bool spriteFlip = animHandler.sprite.FlipH;

        Vector2 launchDir = Vector2.Zero;

        float rotation = 0.0f;
        var animName = animHandler.GetCurrentAnimation();

        if (animName.Find("front_side") != -1)
        {
            launchDir = new Vector2(1.0f, 1.0f);
            
            if (!spriteFlip)
                rotation = 1.0f * Mathf.Pi / 4.0f;
            else
                rotation = 3.0f * Mathf.Pi / 4.0f;
        }
        else if (animName.Find("back_side") != -1)
        {
            launchDir = new Vector2(1.0f, -1.0f);

            if (!spriteFlip)
                rotation = 7.0f * Mathf.Pi / 4.0f;
            else
                rotation = 5.0f * Mathf.Pi / 4.0f;
        }
        else if (animName.Find("front") != -1)
        {
            launchDir = new Vector2(0.0f, 1.0f);
            rotation = 1.0f * Mathf.Pi / 2.0f;
        }
        else if (animName.Find("back") != -1)
        {
            launchDir = new Vector2(0.0f, -1.0f);
            rotation = 3.0f * Mathf.Pi / 2.0f;
        }
        else if (animName.Find("side") != -1)
        {
            launchDir = new Vector2(1.0f, 0.0f);
            if (spriteFlip)
                rotation = Mathf.Pi;
        }

        if (spriteFlip)
        {
            startPos.X = -startPos.X;
            launchDir.X = -launchDir.X;
        }

        inst.Position = startPos + parent.Position;
        inst.Rotation = rotation;
        inst.launchDir = launchDir;
    }
}
