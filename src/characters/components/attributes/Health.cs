using Game.Component;

using Godot;
using System;

public partial class Health : Component
{
    [Signal] public delegate void health_changedEventHandler(float newValue);
    [Export] public float max = 10.0f;

    private float _curr;
    protected float curr
    {
        get => _curr;

        set
        {
            _curr = Mathf.Clamp(value, 0.0f, max);
            EmitSignal(SignalName.health_changed, _curr);
        }
    }

    public override void _Ready()
    {
        base._Ready();
        curr = max;
    }

    public void Use(float cost)
    {
        if (Mathf.IsZeroApprox(cost))
            return;

        if (curr - cost <= 0.0f)
        {
            parent.Dies();
            return;
        }

        curr -= cost;
    }

    public void Restore(float amount)
    {
        if (curr + amount >= max)
            curr = max;
        else
            curr += amount;
    }
}
