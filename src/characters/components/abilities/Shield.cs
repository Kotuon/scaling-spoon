namespace Game.Component;

using Godot;
using System;
using Game.Entity;

/// <summary>
/// TODO: Create hit trigger/effect
/// TODO: if using shield and hit by attack convert to mana (limited amount?)
/// </summary>

public partial class Shield : Ability
{
    public Shield() : base("shield")
    {
        
    }

    public override void Trigger()
    {
        base.Trigger();

        parent.attributes["canMove"] = false;

        animHandler.PlayAnimation("shield_init", mouseRef.mouseDir);
        mouseRef.useMouseDirection = true;
    }

    public override void Update(double delta)
    {
        base.Update(delta);

        if (animHandler.GetCurrentAnimation().Find("shield_init") == -1)
            animHandler.PlayAnimation("shield_idle", mouseRef.mouseDir);
    }

    public override void End()
    {
        base.End();

        parent.attributes["canMove"] = true;
        mouseRef.useMouseDirection = false;
    }
}
