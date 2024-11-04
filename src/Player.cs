namespace Game.Entity.Player;

using Godot;
using System;
using Game.Component;

public partial class Player : CharacterBase
{
    [Export]
    public Move moveComponent;
    [Export]
    public AnimationHandler animationHandler;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        moveComponent.parent = this;
        animationHandler.parent = this;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
