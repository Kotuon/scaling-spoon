namespace Game.Component;

using Godot;
using System;
using Game.Entity;
using System.Numerics;


public partial class Move : Component
{
    // Walking
    [Export]
    public float maxRunSpeed { get; set; } = 400.0f;
    [Export]
    public float maxWalkSpeed { get; set; } = 100.0f;
    public float currMoveSpeed { get; private set; }
    public float lastMoveSpeed { get; private set; }
    [Export]
    public float walkFriction { get; set; } = 0.2f;
    [Export]
    public float slideFriction { get; set; } = 0.02f;
    [Export]
    public float airDrag { get; set; } = 0.01f;
    [Export]
    public float acceleration { get; set; } = 0.25f;
    [Export]
    public float timeToAccelerate { get; set; } = 2.0f;
    [Export]
    public float airControl { get; set; } = 0.5f;

    private float speedAlpha = 0.0f;
    public bool isDown = false;

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
    public float jumpSpeed { get; set; } = -1000.0f;
    private bool hasJumpInput = false;

    private bool _canMove { get; set; } = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        AddChild(coyoteTimer);
        coyoteTimer.WaitTime = coyoteTime;
        coyoteTimer.OneShot = true;
        coyoteTimer.Timeout += () => { canCoyoteJump = false; };

        AddChild(jumpBufferTimer);
        jumpBufferTimer.WaitTime = jumpBufferTime;
        jumpBufferTimer.OneShot = true;
        jumpBufferTimer.Timeout += () => { hasJumpInput = false; };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        Label label = (Label)parent.GetNode("SpeedAlphaLabel");
        label.Text = speedAlpha.ToString();
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
            newVelocity.Y = currVelocity.Y + parent.gravity.Y * (float)delta;
        }

        float inputDirection = ProcessInput();
        if (inputDirection != 0.0f)
        {
            speedAlpha = Mathf.Clamp(speedAlpha +
                                     (acceleration / timeToAccelerate),
                                     0.0f, 1.0f);


            // Quadratic lerp
            float lerp1 = Mathf.Lerp(0.0f, inputDirection * maxWalkSpeed,
                                     speedAlpha);

            float lerp2 = Mathf.Lerp(inputDirection * maxWalkSpeed,
                                     inputDirection * maxRunSpeed,
                                     speedAlpha);

            newVelocity.X = Mathf.Lerp(lerp1, lerp2, speedAlpha);
        }
        else if (parent.IsOnFloor())
        {
            float friction = isDown ? slideFriction : walkFriction;
            newVelocity.X = AddDrag(currVelocity, friction);
        }
        else
        {
            newVelocity.X = AddDrag(currVelocity, airDrag);
        }

        parent.SetVelocity(newVelocity);
    }

    private float AddDrag(Godot.Vector2 currVelocity, float dragAmount)
    {
        speedAlpha = Mathf.Clamp(speedAlpha -
                         (acceleration / timeToAccelerate),
                         0.0f, 1.0f);
        return Mathf.Lerp(currVelocity.X, 0.0f, dragAmount);
    }

    private float ProcessInput()
    {
        float inputDirection = Input.GetAxis("walk_left", "walk_right");
        isDown = (Input.GetActionStrength("down") > 0.0f) ? true : false;

        if (isDown)
        {
            inputDirection = 0.0f;
        }

        return inputDirection;
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
            hasJumpInput = true;
            if (!newOnFloorResult)
                jumpBufferTimer.Start();
        }

        if ((newOnFloorResult || canCoyoteJump) && hasJumpInput)
        {
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
