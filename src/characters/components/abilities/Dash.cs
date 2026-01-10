namespace Game.Component;

using Godot;
using System;

public partial class Dash : Ability
{
    [Export]
    private GpuParticles2D dashParticles;
    [Export]
    public float speed { get; set; } = 800.0f;

    protected float currSpeed
    {
        set
        {
            parent.attributes["currSpeed"] = value;
        }
        get
        {
            var retSpeed = (float)parent.attributes["currSpeed"];

            if (retSpeed > move.maxWalkSpeed)
                SetDashParticles(true);
            else
                SetDashParticles(false);

            return retSpeed;
        }
    }

    public Dash() : base("dash")
    {
        
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (!isActive && !onCooldown && 
            @event.GetActionStrength(abilityName) != 0.0f)
            Trigger();
    }


    public override void Trigger()
    {
        if ((float)parent.attributes["currSpeed"] < move.maxWalkSpeed)
            return;

        base.Trigger();
        if (!isActive) return;

        move.movementOverride = true;
    }

    public override void Update(double delta)
    {
        if (!manaManager.UseMana(cost * (float)delta) || 
            !(bool)parent.attributes["canMove"])
        {
            End();
            return;
        }
        
        base.Update(delta);

        var dir = parent.GetComponent<Controller>().moveInput;

        currSpeed = move.UpdateSpeed(currSpeed, speed, 0.0f, delta, dir);

        var velocity = move.UpdateWalk(currSpeed, speed, delta, dir);

        parent.GetComponent<OffsetCamera>().TriggerOffset(
            velocity.Normalized() * 250.0f, 0.0035f
        );

        parent.SetVelocity(velocity);
        parent.MoveAndSlide();

        animHandler.PlayAnimation("dash", velocity);
    }


    public override void End()
    {
        base.End();

        move.movementOverride = false;
        SetDashParticles(false);

        parent.GetComponent<OffsetCamera>().CancelOffset();
    }

    private void SetDashParticles(bool setValue)
    {
        if (dashParticles.Emitting != setValue)
        {
            dashParticles.Emitting = setValue;
        }
    }
}
