namespace Game.Entity;

using Godot;
using System;

public partial class CharacterBase : CharacterBody2D
{
    [Export] public Godot.Collections.Dictionary attributes;
    
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

    public void AddAttribute(Variant key, Variant value)
    {
        if (attributes.ContainsKey(key))
            return;
        
        attributes.Add(key, value);
    }

    public T GetComponent<T>() where T : class
    {
        foreach (Node child in GetChildren())
        {
            foreach (Node subChild in child.GetChildren())
            {
                if (subChild is T)
                    return subChild as T;
            }
            if (child is T)
                return child as T;
        }

        return default(T);
    }
}
