using Game.Component;

using Godot;
using System;


public partial class ManaManager : Component
{
    
    [Signal]
    public delegate void mana_changedEventHandler(float newValue);
    
    [Export] float max_mana = 100.0f;
    private float m_curr_mana;
    public float curr_mana
    {
        get => m_curr_mana;

        set
        {
            m_curr_mana = Mathf.Clamp(value, 0.0f, max_mana);
            EmitSignal(SignalName.mana_changed, m_curr_mana);
        }
    }

    [Export] public float timeUntilRegen = 1.0f;
    private Timer regenTimer;

    private bool isRegening = false;

    [Export] public float regenRate = 10.0f;

    public override void _Ready()
    {
        base._Ready();

        curr_mana = max_mana / 2.0f;

        regenTimer = new Timer();
        AddChild(regenTimer);
        regenTimer.WaitTime = timeUntilRegen;
        regenTimer.Timeout += () => isRegening = true;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public bool UseMana(float cost)
    {
        if (Mathf.IsZeroApprox(cost))
            return true;
        
        if (curr_mana - cost <= 0.0f)
            return false;
        
        curr_mana -= cost;

        isRegening = false;
        regenTimer.Start();

        return true;
    }

    public void RestoreMana(float restoreAmount)
    {
        if (curr_mana + restoreAmount >= max_mana)
            curr_mana = max_mana;
        else
            curr_mana += restoreAmount;
    }

    private void Regen(float delta)
    {
        if (isRegening)
        {
            curr_mana += regenRate * (float)delta;

            if (curr_mana >= max_mana)
            {
                isRegening = false;
            }
        }
    }
}
