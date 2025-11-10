using System.Reflection.Metadata.Ecma335;

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
    
    private float _currWalkSpeed;
    public float currWalkSpeed
    {
        private set
        {
            _currWalkSpeed = value;
            // if (_currWalkSpeed > maxWalkSpeed)
            //     SetDashParticles(true);
            // else
            //     SetDashParticles(false);
        }
        
        get => _currWalkSpeed;
    }
    public float lastWalkSpeed { get; private set; }
    [Export]
    public float friction { get; set; } = 0.2f;
    [Export]
    public float acceleration { get; set; } = 600.0f;
    [Export]
    public float turnSpeed { get; set; } = 1000000.0f;
    [Export]
    public float brakeSpeed { get; set; } = 1250.0f;
    [Export]
    public AudioStream[] footstepSounds;
    [Export]
    public AudioStreamPlayer2D footstepPlayer;
    public bool _canMove { get; set; } = true;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rng.Randomize();
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
            var inputDirection = parent.GetComponent<Controller>().moveInput;
            UpdateWalk(delta, inputDirection);
        }
        else
        {
            UpdateWalk(delta, Godot.Vector2.Zero);
        }
    }

    private void UpdateSpeed(double delta, Godot.Vector2 direction)
    {
        var dash = parent.GetComponent<Dash>();

        float maxSpeedToUse = dash.isActive ? dash.speed : maxWalkSpeed;
        
        if (direction.LengthSquared() > 0.0f)
        {
            if (currWalkSpeed < maxSpeedToUse)
                currWalkSpeed += acceleration * (float)delta;
            else
                currWalkSpeed = maxSpeedToUse;
        }
        else
        {
            if (!Godot.Mathf.IsEqualApprox(currWalkSpeed, 0.0f))
            {
                currWalkSpeed = (float)Godot.Mathf.Clamp(
                    currWalkSpeed - brakeSpeed * (float)delta,
                    0.0, maxSpeedToUse);
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
