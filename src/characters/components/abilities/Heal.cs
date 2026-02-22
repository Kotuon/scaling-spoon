namespace Game.Component;

using Godot;
using System;

public partial class Heal : Ability
{
    [Export] public float healAmount = 1.0f;
    [Export] public float timeBetweenHeals = 0.25f;
    private float counter;
    private Health _health;
    protected Health health
    {
        private set=> _health = value;

        get
        {
            if (_health == null)
                _health = parent.GetComponent<Health>();

            return _health;
        }
    }

    public Heal() : base("heal")
    {

    }

    public override void _Ready()
    {
        base._Ready();

        cost = healAmount;
    }

    public override void Trigger()
    {
        base.Trigger();
        GD.Print("TRIGGER");
        counter = 0.0f;
    }


    public override void Update(double delta)
    {
        base.Update(delta);

        if (!isActive) return;

        counter -= (float)delta;

        if (counter <= 0.0f)
        {
            health.Restore(healAmount);
            counter = timeBetweenHeals;
        }
    }

}
