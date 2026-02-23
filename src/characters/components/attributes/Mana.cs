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

        curr = 0.0f;

        mana_changed += UpdateAura;
        UpdateAura(curr);
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public bool CanUseMana(float cost)
    {
        if (Mathf.IsZeroApprox(cost))
            return true;

        if (curr - cost >= 0.0f)
        {
            return true;
        }

        return false;
    }

    public bool UseMana(float cost)
    {
        if (CanUseMana(cost))
        {
            curr = Mathf.Clamp(curr - cost, 0.0f, max);
            return true;
        }

        return false;
    }

    public void RestoreMana(float restoreAmount)
    {
        if (curr + restoreAmount >= max)
            curr = max;
        else
            curr += restoreAmount;
    }

    public void ChangeMana(float changeAmount)
    {
        if (changeAmount < 0.0f)
            UseMana(changeAmount * -1.0f);
        else
            RestoreMana(changeAmount);
    }

    private void UpdateAura(float new_mana)
    {
        Sprite2D sprite = parent.GetComponent<Sprite2D>();

        ShaderMaterial shader = sprite.Material as ShaderMaterial;

        float input = (new_mana / max) * 2.5f;

        Tween tween = GetTree().CreateTween();
        tween.TweenMethod(
            new Callable(this, MethodName.SetShaderWidth),
            shader.GetShaderParameter("width"),
            input, 0.25f);

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

    public void SetShaderWidth(float newValue)
    {
        Sprite2D sprite = parent.GetComponent<Sprite2D>();
        ShaderMaterial shader = sprite.Material as ShaderMaterial;
        shader.SetShaderParameter("width", newValue);
    }
}
