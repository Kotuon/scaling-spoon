namespace Game.Component;

using Godot;
using System;
using Game.Entity;

/// <summary>
/// TODO: Create hit trigger/effect
/// </summary>

public partial class Shield : Ability
{
    public override void Trigger()
    {
        base.Trigger();

        move._canMove = false;

        animHandler.PlayAnimation("shield_init", mouseRef.mouseDir);
        mouseRef.useMouseDirection = true;
    }

    public override void Update()
    {
        base.Update();

        if (animHandler.GetCurrentAnimation().Find("shield_init") == -1)
            animHandler.PlayAnimation("shield_idle", mouseRef.mouseDir);
    }

    public override void End()
    {
        base.End();

        move._canMove = true;
        mouseRef.useMouseDirection = false;
    }
}
