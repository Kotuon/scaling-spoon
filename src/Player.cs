namespace Game.Entity.Player;

using Godot;
using System;
using Game.Component;

public partial class Player : CharacterBase
{
    [Export]
    public Move move;

    [Export]
    public AnimationHandler animationHandler;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // moveComponent.parent = this;
        // animationHandler.parent = this;

        foreach (Node child in GetChildren())
        {
            if (child is Component)
            {
                Component comp = (Component)child;
                comp.parent = this;
            }
            if (child is Ability)
            {
                Ability abil = (Ability)child;
                abil.animHandler = animationHandler;
                abil.move = move;
            }
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
