namespace Game.Component;

using Godot;
using System;

public partial class Dash : Ability
{
    [Export]
    private CpuParticles2D dashParticles;
    [Export]
    public float speed { get; set; } = 800.0f;

    public override void Trigger()
    {
        base.Trigger();

        SetDashParticles(true);
    }

    public override void Update()
    {
        base.Update();

        // move.currWalkSpeed
    }


    public override void End()
    {
        base.End();

        SetDashParticles(false);
    }

    private void SetDashParticles(bool setValue)
    {
        if (dashParticles.Emitting != setValue)
        {
            dashParticles.Emitting = setValue;
        }
    }
}
