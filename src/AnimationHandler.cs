namespace Game.Component;

using Godot;
using System;
using Game.Entity;

public partial class AnimationHandler : Component
{
    [Export]
    public AnimationPlayer animationPlayer;
    [Export]
    public Sprite2D sprite;
    [Export]
    public CpuParticles2D landingParticles;
    [Export]
    public float runCutoff { get; set; } = 200;

    private Vector2 lastNonZeroInput = new Vector2(0, 1);

    private bool lastOnFloor = true;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer.Play("front_idle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Vector2 currVelocity = ClipSmallValues(parent.GetRealVelocity());
        UpdateWalkAnimation(currVelocity);
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    private String GetAnimationDirection(Vector2 currVelocity)
    {
        Vector2 direction = currVelocity.Normalized();

        String animationDirection = "front";

        if (direction.Y > 0.1)
        {
            if (Mathf.Abs(direction.X) < 0.1f)
                animationDirection = "front";
            else
                animationDirection = "front_side";
        }
        else if (direction.Y < -0.1)
        {
            if (Mathf.Abs(direction.X) < 0.1f)
                animationDirection = "back";
            else
                animationDirection = "back_side";
        }
        else
            animationDirection = "side";

        return animationDirection;
    }

    private void UpdateWalkAnimation(Vector2 currVelocity)
    {
        if (currVelocity.LengthSquared() > 0.0f)
            lastNonZeroInput = currVelocity.Normalized();


        if (Mathf.IsZeroApprox(currVelocity.LengthSquared()))
        {
            animationPlayer.Play(
                GetAnimationDirection(lastNonZeroInput) + "_idle");
            sprite.FlipH = lastNonZeroInput.X < 0 ? true : false;
        }
        else if (currVelocity.Length() > runCutoff)
        {
            animationPlayer.Play(GetAnimationDirection(currVelocity) + "_run");
            sprite.FlipH = currVelocity.X < 0 ? true : false;
        }
        else
        {
            animationPlayer.Play(GetAnimationDirection(currVelocity) + "_walk");
            sprite.FlipH = currVelocity.X < 0 ? true : false;
        }

    }

    private Vector2 ClipSmallValues(Vector2 inputVec)
    {
        if (Mathf.Abs(inputVec.X) < 0.1f)
            inputVec.X = 0.0f;
        if (Mathf.Abs(inputVec.Y) < 0.1f)
            inputVec.Y = 0.0f;

        return inputVec;
    }
}
