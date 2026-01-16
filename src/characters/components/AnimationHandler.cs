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
    public float runCutoff { get; set; } = 200.0f;
    [Export]
    public float dashCutoff { get; set; } = 400.0f;

    private Vector2 lastNonZeroInput = new Vector2(0, 1);

    private float dirCutoff = 0.25f;

    private bool lastOnFloor = true;
    public bool canAdvance = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer.Play("front_idle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        if (!canAdvance)
        {
            if (animationPlayer.CurrentAnimationPosition >=
                animationPlayer.CurrentAnimationLength)
            {
                canAdvance = true;
                // parent.attributes["canMove"] = true;
        parent.SetAttribute("canMove", true);
                mouseRef.useMouseDirection = false;
            }
        }
        // else if ((bool)parent.attributes["canMove"] && 
        else if (parent.GetAttribute<bool>("canMove") && 
            !parent.GetComponent<Move>().movementOverride)
        {
            Vector2 currVelocity = ClipSmallValues(parent.GetRealVelocity());
            UpdateWalkAnimation(currVelocity);
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
    }

    public String GetCurrentAnimation()
    {
        return animationPlayer.CurrentAnimation;
    }
    public void PlayAnimation(String animName, Vector2 inputDir)
    {
        var dir = GetAnimationDirection(inputDir);
        
        sprite.FlipH = inputDir.X > 0 ? false : true;

        if (Mathf.IsZeroApprox(inputDir.LengthSquared()))
        {
            dir = GetAnimationDirection(lastNonZeroInput);
            sprite.FlipH = lastNonZeroInput.X > 0 ? false : true;
        }
        
        animationPlayer.Play(dir + "_" + animName);
    }

    private String GetAnimationDirection(Vector2 currVelocity)
    {
        Vector2 direction = currVelocity.Normalized();

        String animationDirection = "front";

        if (!Mathf.IsZeroApprox(currVelocity.LengthSquared()))
            lastNonZeroInput = currVelocity.Normalized();

        if (direction.Y > dirCutoff)
        {
            if (Mathf.Abs(direction.X) < 0.1f)
                animationDirection = "front";
            else
                animationDirection = "front_side";
        }
        else if (direction.Y < -dirCutoff)
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
        float speed = currVelocity.Length();

        var mouseDirection = mouseRef.GlobalPosition - parent.GlobalPosition;

        var currDirection = mouseRef.useMouseDirection 
            ? GetAnimationDirection(mouseDirection)
            : GetAnimationDirection(currVelocity);

        if (Mathf.IsZeroApprox(speed))
        {
            animationPlayer.Play(
                GetAnimationDirection(lastNonZeroInput) + "_idle");
            sprite.FlipH = lastNonZeroInput.X < 0 ? true : false;
        }
        // else if (speed > dashCutoff)
        // {
        //     animationPlayer.Play(GetAnimationDirection(currVelocity) + "_dash");
        //     sprite.FlipH = currVelocity.X < 0 ? true : false;
        // }
        else if (speed > runCutoff)
        {
            animationPlayer.Play(currDirection + "_run");
            // sprite.FlipH = currVelocity.X < 0 ? true : false;

            sprite.FlipH = mouseRef.useMouseDirection ? mouseDirection.X < 0
                : currVelocity.X < 0;
        }
        else
        {
            animationPlayer.Play(currDirection + "_walk");
            // sprite.FlipH = currVelocity.X < 0 ? true : false;

            sprite.FlipH = mouseRef.useMouseDirection ? mouseDirection.X < 0
                : currVelocity.X < 0;
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
