namespace Game.Entity;

using Godot;
using Microsoft.VisualBasic;
using System;
using System.Runtime.InteropServices;


public partial class CharacterBase : CharacterBody2D, IDamageable
{
    [Export] protected Godot.Collections.Dictionary attributes;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddAttribute("canMove", true);
        AddAttribute("currSpeed", 0.0f);
        AddAttribute("health", 10.0f);
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

    public T GetAttribute<[MustBeVariant]T>(Variant key)
    {
        Variant outValue;

        if (attributes.TryGetValue(key, out outValue))
            return outValue.As<T>();

        GD.PushError("Key doesn't exist");

        return default(T);
    }

    public void SetAttribute(Variant key, Variant newValue)
    {
        if (attributes[key].VariantType == newValue.VariantType)
            attributes[key] = newValue;
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

    public void Damage(float amount)
    {
        // attributes["health"] = (float)attributes["health"] - amount;
        // attributes["health"] = attributes["health"].As<float>() - amount;
        SetAttribute("health", GetAttribute<float>("health") - amount);

        float health = GetAttribute<float>("health");
        
        if (GetAttribute<float>("health") <= 0.0f)
        {
            Dies();
        }
    }

    public void Dies()
    {
        Visible = false;
    }
}
