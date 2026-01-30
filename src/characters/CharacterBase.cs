namespace Game.Entity;

using Game.Component;
using Godot;
using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;


public partial class CharacterBase : CharacterBody2D, IDamageable
{
    [Signal] public delegate void collisionEventHandler();
    [Signal] public delegate void damagedEventHandler(float damageAmount);
    // [Export] protected Godot.Collections.Dictionary attributes;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public T GetComponent<T>() where T : class
    {
        foreach (Node child in GetChildren())
        {
            if (child is T)
                return child as T;
            foreach (Node subChild in child.GetChildren())
            {
                if (subChild is T)
                    return subChild as T;
            }
        }

        return default(T);
    }

    public T GetComponent<T>(string name) where T : class
    {
        foreach (Node child in GetChildren())
        {
            if (child is T && child.Name.Equals(name))
                return child as T;
            foreach (Node subChild in child.GetChildren())
            {
                if (subChild is T && subChild.Name.Equals(name))
                    return subChild as T;
            }

        }

        return default(T);
    }

    public virtual void Damage(float amount)
    {
        Health health = GetComponent<Health>();
        if (health == null) return;

        health.Use(amount);

        EmitSignal(SignalName.damaged, amount);
    }

    public virtual void Dies()
    {
        Visible = false;
    }
}
