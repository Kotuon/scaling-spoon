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
    [Signal] public delegate void startShieldEventHandler();
    [Signal] public delegate void endShieldEventHandler();

    public Shield() : base("shield")
    {

    }

    public override void _Ready()
    {
        base._Ready();

        parent.damaged += WasDamaged;
    }


    public override void Trigger()
    {
        base.Trigger();

        move.canMove = false;

        animHandler.PlayAnimation("shield_init", mouseRef.mouseDir);
        mouseRef.useMouseDirection = true;

        EmitSignal(SignalName.startShield);
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

        animHandler.PlayAnimation("shield_end", mouseRef.mouseDir);
        animHandler.canAdvance = false;

        EmitSignal(SignalName.endShield);
    }

    private void WasDamaged(float damageAmount)
    {
        if (!isActive) return;

        Mana mana = parent.GetComponent<Mana>();
        mana.RestoreMana(10.0f * damageAmount);
    }
}
