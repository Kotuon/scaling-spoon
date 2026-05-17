namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class Dash : Ability
{
    [Export]
    private GpuParticles2D dashParticles;
    [Export]
    public float speed { get; set; } = 800.0f;
    [Export]
    public float updateCost = 0.1f;
    [Export]
    public float thresholdPercent = 0.3f;

    private float _currSpeed;
    protected float currSpeed
    {
        set
        {
            _currSpeed = value;
            move.currWalkSpeed = _currSpeed;
        }
        get
        {
            var retSpeed = move.currWalkSpeed;

            if (retSpeed > move.maxWalkSpeed * thresholdPercent)
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
        if (move.currWalkSpeed < move.maxWalkSpeed * thresholdPercent)
            return;

        var wasActive = isActive;

        base.Trigger();
        if (wasActive && !isActive)
        {
            StartCooldown();
            End();
        }
        else if (!isActive) return;

        currSpeed = move.maxWalkSpeed;
        move.movementOverride = true;
    }

    public override void Update(double delta)
    {
        float frameCost = (float)delta * updateCost;

        if (!move.canMove || !manaManager.CanUseMana(frameCost))
        {
            End();
            return;
        }
        manaManager.UseMana(frameCost);

        base.Update(delta);

        var dir = parent.GetComponent<Controller>().moveInput;

        currSpeed = move.UpdateSpeed(currSpeed, speed, 0.0f, delta, dir);

        var velocity = move.UpdateWalk(currSpeed, speed, delta, dir);

        // parent.GetComponent<OffsetCamera>().TriggerOffset(
        //     velocity.Normalized() * 750.0f, 0.0035f
        // );
        parent.GetComponent<OffsetCamera>().TriggerOffset(
            velocity.Normalized() * 350.0f, 0.75f
        );

        parent.SetVelocity(velocity);

        var collision = parent.MoveAndCollide(parent.Velocity * (float)delta);
        if (collision != null)
        {
            parent.EmitSignal(CharacterBase.SignalName.collision);
        }

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
