namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class Ability : Component
{
    public string abilityName { get; set; } = "";

    // Cooldown stuff
    [Export]
    public float cooldown { get; set; } = 0.1f;
    private Timer cooldownTimer;
    public bool onCooldown = false;

    public bool isActive { private set; get; } = false;

    private AnimationHandler _animHandler;
    public AnimationHandler animHandler
    {
        private set => _animHandler = value;

        get
        {
            if (_animHandler == null)
                _animHandler = parent.GetComponent<AnimationHandler>();
            return _animHandler;
        }
    }

    private Move _move;
    public Move move
    {
        private set => _move = value;

        get
        {
            if (_move == null)
                _move = parent.GetComponent<Move>();
            return _move;
        }
    }

    public new CharacterBase parent
    {
        protected set => _parent = value;

        get
        {
            if (_parent == null)
                _parent = GetNode<CharacterBase>("../..");
            return _parent;
        }
    }

    private AbilityManager _ablManager;
    public AbilityManager ablManager
    {
        private set => _ablManager = value;

        get
        {
            if (_ablManager == null)
                _ablManager = GetNode<AbilityManager>("..");
            return _ablManager;
        }
    }

    private Mana _manaManager;
    public Mana manaManager
    {
        private set => _manaManager = value;

        get
        {
            if (_manaManager == null)
                _manaManager = parent.GetComponent<Mana>();
            return _manaManager;
        }
    }

    [Export] public float cost = 1.0f;

    public Ability(string abilityName_)
    {
        abilityName = abilityName_;
    }

    public override void _Ready()
    {
        if (Mathf.IsZeroApprox(cooldown))
            return;

        cooldownTimer = new Timer();
        AddChild(cooldownTimer);

        cooldownTimer.WaitTime = cooldown;
        cooldownTimer.Timeout += EndCooldown;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (isActive)
            Update(delta);
    }

    public virtual void Pressed()
    {
        if (!isActive && !onCooldown)
            Trigger();
    }

    public virtual void Released()
    {
        if (!isActive) return;

        StartCooldown();
        End();
    }


    public virtual void Trigger()
    {
        isActive = manaManager.UseMana(cost);
    }

    public virtual void Update(double delta)
    {

    }

    public virtual void End()
    {
        isActive = false;
    }

    public void StartCooldown()
    {
        if (Mathf.IsZeroApprox(cooldown))
            return;

        onCooldown = true;
        cooldownTimer.Start();
    }

    private void EndCooldown()
    {
        onCooldown = false;
    }
}
