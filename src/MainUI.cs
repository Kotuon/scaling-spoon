using Game.Component;
using Game.Entity;

using Godot;
using System;
using System.ComponentModel;

public partial class MainUI : Control
{
    [Export] public float progressBarTimeToChange = 0.25f;

    private CharacterBase _parent;
    public CharacterBase parent
    {
        protected set => _parent = value;

        get
        {
            if (_parent == null)
                _parent = GetNode<CharacterBase>("../..");
            return _parent;
        }
    }

    private Camera2D _cameraRef;
    public Camera2D cameraRef
    {
        private set => _cameraRef = value;

        get
        {
            if (_cameraRef == null)
                _cameraRef = parent.GetComponent<Camera2D>();
            return _cameraRef;
        }
    }

    private ProgressBar _manaBar;
    public ProgressBar manaBar
    {
        private set => _manaBar = value;

        get
        {
            if (_manaBar == null)
                _manaBar = GetNode<ProgressBar>("ManaBar");
            return _manaBar;
        }
    }
    private ProgressBar _healthBar;
    public ProgressBar healthBar
    {
        private set => _healthBar = value;

        get
        {
            if (_healthBar == null)
                _healthBar = GetNode<ProgressBar>("Locked/HealthBar");
            return _healthBar;
        }
    }
    private CanvasLayer _locked;
    public CanvasLayer locked
    {
        private set => _locked = value;

        get
        {
            if (_locked == null)
                _locked = GetNode<CanvasLayer>("Locked");
            return _locked;
        }
    }

    public override void _Ready()
    {
        base._Ready();

        Mana mana = parent.GetComponent<Mana>();
        Health health = parent.GetComponent<Health>();

        mana.mana_changed += UpdateManaBar;
        health.health_changed += UpdateHealthBar;

        manaBar.MaxValue = mana.max;
        healthBar.MaxValue = health.max;
    }


    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    private void TweenProgressBarValue(ProgressBar bar, float newValue)
    {
        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(
            bar, "value", newValue, progressBarTimeToChange);
    }

    private void UpdateManaBar(float newValue)
    {
        TweenProgressBarValue(manaBar, newValue);
    }
    private void UpdateHealthBar(float newValue)
    {
        TweenProgressBarValue(healthBar, newValue);
    }
}
