namespace Game.Component;

using Godot;
using System;
using Game.Entity;
using System.Numerics;


public partial class Move : Component
{
    // Walking
    [Export]
    public float maxWalkSpeed { get; set; } = 400.0f;
    public float currWalkSpeed { get; private set; }
    public float lastWalkSpeed { get; private set; }
    [Export]
    public float friction { get; set; } = 0.2f;
    [Export]
    public float acceleration { get; set; } = 0.25f;

    // Jumping
    //// Coyote Time
    private Timer coyoteTimer = new Timer();
    [Export]
    public float coyoteTime { get; set; } = 0.2f;
    private bool canCoyoteJump = false;
    //// Jump Buffering
    private Timer jumpBufferTimer = new Timer();
    [Export]
    public float jumpBufferTime { get; set; } = 0.1f;
    //// General
    [Export]
    public float jumpSpeed { get; set; } = -500.0f;
    private bool hasJumpInput = false;

    private bool _canMove { get; set; } = true;
    public CharacterBase parent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddChild(coyoteTimer);
        coyoteTimer.WaitTime = coyoteTime;
        coyoteTimer.OneShot = true;
        coyoteTimer.Timeout += () => { canCoyoteJump = false; GD.Print("Coyote timer"); };

        AddChild(jumpBufferTimer);
        jumpBufferTimer.WaitTime = jumpBufferTime;
        jumpBufferTimer.OneShot = true;
        jumpBufferTimer.Timeout += () => { hasJumpInput = false; GD.Print("Jump Buffer"); };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (_canMove)
        {
            UpdateWalk(delta);
            UpdateJump();
        }
    }

    private void UpdateWalk(double delta)
    {
        Godot.Vector2 newVelocity = new Godot.Vector2(0.0f, 0.0f);
        Godot.Vector2 currVelocity = parent.GetRealVelocity();

        if (!parent.IsOnFloor())
        {
            newVelocity.Y =
                currVelocity.Y +
                parent.GetGravity().Y * (float)delta;

        }

        float inputDirection = Input.GetAxis("walk_left", "walk_right");
        if (inputDirection != 0.0f)
        {
            newVelocity.X = Mathf.Lerp(currVelocity.X,
                                       inputDirection * maxWalkSpeed,
                                       acceleration);
        }
        else if (parent.IsOnFloor())
        {
            newVelocity.X = Mathf.Lerp(currVelocity.X, 0.0f, friction);
        }

        parent.SetVelocity(newVelocity);
    }

    private void UpdateJump()
    {
        bool lastOnFloorResult = parent.IsOnFloor();
        bool newOnFloorResult = parent.MoveAndSlide();

        if (!newOnFloorResult && lastOnFloorResult)
        {
            canCoyoteJump = true;
            coyoteTimer.Start();
        }

        if (Input.IsActionJustPressed("jump"))
        {
            GD.Print("Jump");
            hasJumpInput = true;
            if (!newOnFloorResult)
            {
                jumpBufferTimer.Start();
            }
        }

        if ((newOnFloorResult || canCoyoteJump) && hasJumpInput)
        {
            GD.Print("Jump set velocity");
            hasJumpInput = false;
            canCoyoteJump = false;

            Godot.Vector2 jumpVelocity =
                new Godot.Vector2(parent.GetRealVelocity().X, jumpSpeed);
            parent.SetVelocity(jumpVelocity);
            parent.MoveAndSlide();
            // TODO: time in air stuff
        }
    }
}