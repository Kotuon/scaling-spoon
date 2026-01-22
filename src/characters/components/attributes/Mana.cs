using Game.Component;

using Godot;
using System;


public partial class Mana : Component
{
    [Signal]
    public delegate void mana_changedEventHandler(float newValue);
    
    [Export] public float max = 100.0f;
    private float _curr;
    public float curr
    {
        get => _curr;

        set
        {
            _curr = Mathf.Clamp(value, 0.0f, max);
            EmitSignal(SignalName.mana_changed, _curr);
        }
    }

    public override void _Ready()
    {
        base._Ready();

        // curr_mana = max_mana / 2.0f;
        curr = 0.0f;

        mana_changed += UpdateAura;
        UpdateAura(curr);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public bool UseMana(float cost)
    {
        if (Mathf.IsZeroApprox(cost))
            return true;
        
        if (curr - cost <= 0.0f)
            return false;
        
        curr -= cost;

        return true;
    }

    public void RestoreMana(float restoreAmount)
    {
        if (curr + restoreAmount >= max)
            curr = max;
        else
            curr += restoreAmount;
    }

    private void UpdateAura(float new_mana)
    {
        Sprite2D sprite = parent.GetComponent<Sprite2D>();

        ShaderMaterial shader = sprite.Material as ShaderMaterial;

        float input = (new_mana / max) * 2.5f;

        shader.SetShaderParameter("width", input);

        GpuParticles2D particles = parent.GetComponent<GpuParticles2D>();

        if (new_mana > 0.0f)
        {
            particles.Emitting = true;
            particles.AmountRatio = new_mana / max;
        }
        else
        {
            particles.Emitting = false;
        }
    }
}
