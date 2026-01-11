using Game.Component;

using Godot;
using System;

public partial class ManaManager : Component
{
    [Export] float max_mana = 100.0f;
    private float m_curr_mana;
    public float curr_mana
    {
        get
        {
            return m_curr_mana;
        }
        set
        {
            m_curr_mana = Mathf.Clamp(value, 0.0f, max_mana);
            manaBar.Value = m_curr_mana;
        }
    }

    [Export] public float timeUntilRegen = 1.0f;
    private Timer regenTimer;

    private bool isRegening = false;

    [Export] public float regenRate = 10.0f;

    private ProgressBar _manaBar;
    public ProgressBar manaBar
    {
        private set => _manaBar = value;

        get
        {
            if (_manaBar == null)
                _manaBar = GetNode<ProgressBar>("../Camera2D/Control/ManaBar");
            return _manaBar;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        curr_mana = max_mana;

        regenTimer = new Timer();
        AddChild(regenTimer);
        regenTimer.WaitTime = timeUntilRegen;
        regenTimer.Timeout += ShouldRegen;

        manaBar.MaxValue = max_mana;
        manaBar.MinValue = 0.0f;
        manaBar.Value = curr_mana;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (isRegening)
        {
            curr_mana += regenRate * (float)delta;

            if (curr_mana >= max_mana)
            {
                isRegening = false;
            }
        }
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

    private void ShouldRegen()
    {
        isRegening = true;
    }
}
