namespace Game.Component;

using Game.Entity;
using Godot;

public partial class Move : Ability
{
    // Walking
    [Export]
    public float maxWalkSpeed { get; set; } = 350.0f;

    private float _currWalkSpeed;

    [Export]
    public float currWalkSpeed
    {
        set { _currWalkSpeed = value; }
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
    public float brakeSpeed { get; set; } = 750.0f;

    [Export]
    public float slideBrakeSpeed { get; set; } = 325.0f;

    [Export]
    public AudioStream[] footstepSounds;

    [Export]
    public AudioStreamPlayer2D footstepPlayer;

    private RandomNumberGenerator rng = new RandomNumberGenerator();

    [Export]
    public bool movementOverride = false;

    [Export]
    public bool canMove = true;

    [Export]
    private bool playSound = true;

    [Export]
    private bool ignorePlayingFootstep = false;

    public Move()
        : base("move") { }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        rng.Randomize();
    }

    public override void _Input(InputEvent @event) { }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (movementOverride)
            return;

        if (canMove)
        {
            var inputDirection = parent.GetComponent<Controller>().moveInput;
            UpdateWalk(brakeSpeed, delta, inputDirection);
        }
        else
        {
            UpdateWalk(slideBrakeSpeed, delta, Vector2.Zero);
        }
    }

    public float UpdateSpeed(
        float currSpeed,
        float maxSpeed,
        float slowSpeed,
        double delta,
        Vector2 direction
    )
    {
        if (direction.LengthSquared() > 0.0f)
        {
            Vector2 normVel = parent.Velocity.Normalized();
            float dot = normVel.Dot(direction);
            if (dot < -0.7f)
            {
                GD.Print(dot);
                currSpeed -= acceleration * (float)delta;
            }
            else
            {
                if (currSpeed < maxSpeed)
                    currSpeed += acceleration * (float)delta;
                else
                    currSpeed = maxSpeed;
            }
        }
        else
        {
            if (!Mathf.IsEqualApprox(currSpeed, 0.0f))
            {
                currSpeed = (float)
                    Mathf.Clamp(
                        currSpeed - slowSpeed * (float)delta,
                        0.0,
                        maxSpeed
                    );
            }
            else
                currSpeed = 0.0f;
        }

        return currSpeed;
    }

    private void UpdateSpeed(float slowSpeed, double delta, Vector2 direction)
    {
        currWalkSpeed = UpdateSpeed(
            currWalkSpeed,
            maxWalkSpeed,
            slowSpeed,
            delta,
            direction
        );
    }

    public Vector2 GetNewVelocity(float currSpeed, Vector2 direction)
    {
        Vector2 newVelocity;
        Vector2 currVelocity = parent.Velocity;

        newVelocity =
            (currVelocity + (direction * turnSpeed)).Normalized() * currSpeed;

        return newVelocity;
    }

    private void UpdateWalk(float slowSpeed, double delta, Vector2 direction)
    {
        UpdateSpeed(slowSpeed, delta, direction);
        Vector2 newVelocity = GetNewVelocity(currWalkSpeed, direction);

        if (
            playSound
            && !Mathf.IsZeroApprox(currWalkSpeed)
            && currWalkSpeed <= maxWalkSpeed
        )
        {
            playFootstepSound();
        }

        parent.SetVelocity(newVelocity.LimitLength(maxWalkSpeed));

        var collision = parent.MoveAndCollide(parent.Velocity * (float)delta);
        if (collision != null)
        {
            parent.EmitSignal(CharacterBase.SignalName.Collision);

            if (collision.GetCollider() is CharacterBase)
            {
                (collision.GetCollider() as CharacterBase).EmitSignal(
                    CharacterBase.SignalName.Collision
                );
            }
        }
    }

    public void playFootstepSound()
    {
        if (footstepPlayer == null)
            return;

        if (footstepPlayer.Playing && !ignorePlayingFootstep)
            return;

        if (footstepSounds.Length == 0)
            return;

        int randomIndex = rng.RandiRange(0, footstepSounds.Length - 1);
        footstepPlayer.Stream = footstepSounds[randomIndex];
        footstepPlayer.Play();
    }

    // For animations
    public void SetCurrSpeed(float new_speed)
    {
        currWalkSpeed = new_speed;
    }
}
