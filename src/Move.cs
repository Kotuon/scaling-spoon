using System.Reflection.Metadata.Ecma335;

namespace Game.Component;

using Godot;
using System;
using Game.Entity;
using System.Numerics;


public partial class Move : Ability
{
    // Walking
    [Export]
    public float maxWalkSpeed { get; set; } = 350.0f;
    
    public float currWalkSpeed
    {
        set
        {
            parent.attributes["currSpeed"] = value;
        }
        
        get => (float)parent.attributes["currSpeed"];
    }
    public float lastWalkSpeed { get; private set; }
    [Export]
    public float friction { get; set; } = 0.2f;
    [Export]
    public float acceleration { get; set; } = 600.0f;
    [Export]
    public float turnSpeed { get; set; } = 1000000.0f;
    [Export]
    public float brakeSpeed { get; set; } = 750.0f;
    [Export]
    public float slideBrakeSpeed { get; set; } = 325.0f;
    [Export]
    public AudioStream[] footstepSounds;
    [Export]
    public AudioStreamPlayer2D footstepPlayer;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    public bool movementOverride = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rng.Randomize();
    }

    public override void _Input(InputEvent @event)
    {
    }


    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (movementOverride)
            return;

        if ((bool)parent.attributes["canMove"])
        {
            var inputDirection = parent.GetComponent<Controller>().moveInput;
            UpdateWalk(brakeSpeed, delta, inputDirection);
        }
        else
        {
            UpdateWalk(slideBrakeSpeed, delta, Godot.Vector2.Zero);
            GD.Print("Slow");
        }
    }

    public float UpdateSpeed(float currSpeed, float maxSpeed, float slowSpeed,
        double delta, Godot.Vector2 direction)
    {
        if (direction.LengthSquared() > 0.0f)
        {
            if (currSpeed < maxSpeed)
                currSpeed += acceleration * (float)delta;
            else
                currSpeed = maxSpeed;
        }
        else
        {
            if (!Mathf.IsEqualApprox(currSpeed, 0.0f))
            {
                currSpeed = (float)Mathf.Clamp(
                    currSpeed - slowSpeed * (float)delta,
                    0.0, maxSpeed);
            }
            else
                currSpeed = 0.0f;
        }

        return currSpeed;
    }

    private void UpdateSpeed(float slowSpeed, double delta, Godot.Vector2 direction)
    {
        var dash = parent.GetComponent<Dash>();

        float maxSpeedToUse = dash.isActive ? dash.speed : maxWalkSpeed;

        currWalkSpeed = UpdateSpeed(currWalkSpeed, maxSpeedToUse, slowSpeed,
            delta, direction);
    }

    public Godot.Vector2 UpdateWalk(float currSpeed, float maxSpeed, 
        double delta, Godot.Vector2 direction)
    {
        Godot.Vector2 newVelocity;
        Godot.Vector2 currVelocity = parent.GetRealVelocity();

        if (currVelocity.Normalized() == (direction.Normalized() * -1.0f))
        {
            direction += direction.Orthogonal() * turnSpeed;
        }

        newVelocity = (currVelocity + (direction * turnSpeed)).Normalized() *
            currSpeed;

        return newVelocity;
    }

    private void UpdateWalk(float slowSpeed, double delta, Godot.Vector2 direction)
    {
        UpdateSpeed(slowSpeed, delta, direction);
        Godot.Vector2 newVelocity = UpdateWalk(currWalkSpeed, maxWalkSpeed,
            delta, direction);

        if (currWalkSpeed != 0.0 && currWalkSpeed <= maxWalkSpeed)
            playFootstepSound();

        parent.SetVelocity(newVelocity);
        parent.MoveAndSlide();
    }

    private void playFootstepSound()
    {
        if (footstepPlayer.Playing)
            return;

        if (footstepSounds.Length == 0)
            return;

        int randomIndex = rng.RandiRange(0, footstepSounds.Length - 1);
        footstepPlayer.Stream = footstepSounds[randomIndex];
        footstepPlayer.Play();
    }
}
