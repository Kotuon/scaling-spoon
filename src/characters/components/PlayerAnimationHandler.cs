namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class PlayerAnimationHandler : AnimationHandler
{
    [Export] public int currFrame = 0;

    public void set_frame(int newFrame)
    {
        currFrame = newFrame;
        sprite.FrameCoords = new Vector2I(currFrame, (int)currDir);
    }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer.Play(animationLibrary + "/idle_default");
        sprite.TextureChanged += UpdateShaderForTexture;

        animationPlayer.GetAnimationLibraryList();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }


    protected override void UpdateWalkAnimation(Vector2 currVelocity)
    {
        float speed = currVelocity.Length();

        var mouseDirection = mouseRef.GlobalPosition - parent.GlobalPosition;

        currDir = mouseRef.useMouseDirection
            ? GetAnimationDirection(mouseDirection)
            : GetAnimationDirection(currVelocity);

        if (Mathf.IsZeroApprox(speed))
        {
            currDir = GetAnimationDirection(lastNonZeroInput);
            animationPlayer.Play(animationLibrary + "/idle_default");
            flip = lastNonZeroInput.X < 0 ? true : false;
        }
        else
        {
            animationPlayer.Play(animationLibrary + "/walk");

            flip = mouseRef.useMouseDirection ? mouseDirection.X < 0
                : currVelocity.X < 0;
        }
    }

    private void UpdateShaderForTexture()
    {
        ShaderMaterial shader = sprite.Material as ShaderMaterial;

        if (shader == null)
            return;

        shader.SetShaderParameter("hframes", sprite.Hframes);
        shader.SetShaderParameter("vframes", sprite.Vframes);
    }
}
