namespace Game.Component;

using Godot;
using System;
using Game.Entity;
using System.Numerics;


public partial class Move : Component
{
    // Walking
    [Export]
    public float maxWalkSpeed { get; set; } = 350.0f;
    public float currWalkSpeed { get; private set; }
    public float lastWalkSpeed { get; private set; }
    [Export]
    public float friction { get; set; } = 0.2f;
    [Export]
    public float acceleration { get; set; } = 600.0f;
    [Export]
    public float turnSpeed { get; set; } = 1000000.0f;
    [Export]
    public float brakeSpeed { get; set; } = 1250.0f;

    private bool _canMove { get; set; } = true;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
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
            Godot.Vector2 inputDirection = Input.GetVector("walk_left",
                "walk_right", "walk_up", "walk_down");
            UpdateWalk(delta, inputDirection);
        }
    }

    private void UpdateSpeed(double delta, Godot.Vector2 direction)
    {
        if (direction.LengthSquared() > 0.0f)
        {
            if (currWalkSpeed < maxWalkSpeed)
                currWalkSpeed += acceleration * (float)delta;
            else
                currWalkSpeed = maxWalkSpeed;
        }
        else
        {
            if (!Godot.Mathf.IsEqualApprox(currWalkSpeed, 0.0f))
            {
                currWalkSpeed = (float)Godot.Mathf.Clamp(
                    currWalkSpeed - brakeSpeed * (float)delta,
                    0.0, maxWalkSpeed);
            }
            else
                currWalkSpeed = 0.0f;
        }
    }

    private void UpdateWalk(double delta, Godot.Vector2 direction)
    {
        Godot.Vector2 newVelocity;
        Godot.Vector2 currVelocity = parent.GetRealVelocity();

        UpdateSpeed(delta, direction);

        if (currVelocity.Normalized() == (direction.Normalized() * -1.0f))
        {
            direction += direction.Orthogonal() * turnSpeed;
        }

        newVelocity = (currVelocity + (direction * turnSpeed)).Normalized() *
            currWalkSpeed;

        parent.SetVelocity(newVelocity);
        parent.MoveAndSlide();
    }

}
