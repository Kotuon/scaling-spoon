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

        Godot.Vector2 inputDirection = Input.GetVector("walk_left",
            "walk_right", "walk_up", "walk_down");

        animHandler.PlayAnimation(inputDirection, "shield_init");
        mouseRef.useMouseDirection = true;
    }

    public override void Update()
    {
        base.Update();

        Godot.Vector2 inputDirection = Input.GetVector("walk_left",
            "walk_right", "walk_up", "walk_down");

        if (animHandler.GetCurrentAnimation().Find("shield_init") == -1)
            animHandler.PlayAnimation(inputDirection, "shield_idle");
    }

    public override void End()
    {
        base.End();

        move._canMove = true;
        mouseRef.useMouseDirection = false;
    }
}
