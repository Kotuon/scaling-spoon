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

    private bool lastOnFloor = true;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        animationPlayer.Play("idle");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        bool currOnFloor = parent.IsOnFloor();

        Godot.Vector2 currVelocity = ClipSmallValues(parent.GetRealVelocity());

        if (animationPlayer.CurrentAnimation != "land" ||
            (animationPlayer.CurrentAnimation == "land" &&
                !animationPlayer.IsPlaying()))
        {
            if (currOnFloor)
            {
                UpdateWalkAnimation(currVelocity);
            }
            else if (lastOnFloor)
            {
                animationPlayer.Play("jump");
            }
            else if (!lastOnFloor && currVelocity.Y > 0.0f)
            {
                if (currOnFloor && animationPlayer.CurrentAnimation != "land")
                {
                    animationPlayer.Play("land");
                    GD.Print("land");
                }
                else if (animationPlayer.CurrentAnimation != "fall")
                {
                    animationPlayer.Play("fall");
                }
            }
        }

        lastOnFloor = currOnFloor;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        bool currOnFloor = parent.IsOnFloor();
        Godot.Vector2 currVelocity = ClipSmallValues(parent.GetRealVelocity());

        if (!currOnFloor && !lastOnFloor && currVelocity.Y > 0.0f)
        {
            float time = animationPlayer.GetAnimation("land").Length / 4.0f;

            float distance = (currVelocity.Y * time) +
                0.5f * (parent.GetGravity().Y * (time * time));

            var spaceState = GetWorld2D().DirectSpaceState;

            var query =
                PhysicsRayQueryParameters2D.Create(parent.Position,
                    parent.Position + (Vector2.Down * distance));

            var result = spaceState.IntersectRay(query);
            if (result.Count > 0)
            {
                animationPlayer.Play("land");
                GD.Print("Land");
            }
        }
    }

    private void UpdateWalkAnimation(Vector2 currVelocity)
    {
        if (currVelocity.X > 5.0f)
        {
            sprite.FlipH = false;

            if (currVelocity.X > 100.0f)
            {
                animationPlayer.Play("run");
            }
            else
            {
                animationPlayer.Play("walk");
            }
        }
        else if (currVelocity.X < -5.0f)
        {
            sprite.FlipH = true;

            if (currVelocity.X < -100.0f)
            {
                animationPlayer.Play("run");
            }
            else
            {
                animationPlayer.Play("walk");
            }
        }
        else
        {
            animationPlayer.Play("idle");
        }
    }

    private Godot.Vector2 ClipSmallValues(Godot.Vector2 inputVec)
    {
        if (Mathf.Abs(inputVec.X) < 0.1f)
        {
            inputVec.X = 0.0f;
        }
        if (Mathf.Abs(inputVec.Y) < 0.1f)
        {
            inputVec.Y = 0.0f;
        }

        return inputVec;
    }
}
