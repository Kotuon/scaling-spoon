namespace Game.Entity;

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
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void Damage(float amount)
    {
        base.Damage(amount);

        animationHandler.PlayAnimation("hit", Vector2.Zero);
        animationHandler.canAdvance = false;
        move.currWalkSpeed = 0.0f;
    }


    override public void Dies()
    {
        GetTree().CallDeferred(SceneTree.MethodName.ReloadCurrentScene);
        // GetTree().ReloadCurrentScene();
        GD.Print("Dies");
    }
}
