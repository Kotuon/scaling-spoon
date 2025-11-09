namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class Ability : Component
{
    [Export]
    public String abilityName { get; set; } = "";

    // Cooldown stuff
    [Export]
    public float cooldown { get; set; } = 0.1f;
    private Timer cooldownTimer;
    private bool onCooldown = false;

    private bool isActive = false;

    public AnimationHandler animHandler { get; set; }
    public Move move { get; set; }

    public override void _Ready()
    {
        cooldownTimer = new Timer();
        AddChild(cooldownTimer);

        cooldownTimer.WaitTime = cooldown;
        cooldownTimer.Timeout += EndCooldown;
    }

    public override void _Input(InputEvent @event)
    {
        base._Input(@event);

        if (@event.IsActionPressed(abilityName))
        {
            if (!onCooldown)
            {
                Trigger();
            }
        }
        if (@event.IsActionReleased(abilityName))
        {
            if (isActive)
            {
                StartCooldown();
                End();
            }
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (isActive)
            Update();
    }


    public virtual void Trigger()
    {
        isActive = true;
    }

    public virtual void Update()
    {

    }

    public virtual void End()
    {
        isActive = false;
    }

    private void StartCooldown()
    {
        onCooldown = true;
        cooldownTimer.Start();
    }

    private void EndCooldown()
    {
        onCooldown = false;
    }
}
