namespace Game.Component;

using Game.Entity;
using Godot;

public partial class Roll : Ability
{
    [Export]
    protected Curve RollSpeedCurve;

    [Export]
    protected float MinSpeed = 125.0f;
    private Vector2 roll_dir = Vector2.Zero;
    private float speed_buffer = 0.0f;

    ////////////////////////////////////////////////////////////////////////////////
    public Roll()
        : base("roll") { }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        base._Ready();

        animHandler.animationPlayer.AnimationFinished += (StringName s) =>
        {
            if (s.ToString().Contains(abilityName))
            {
                End();
            }
        };
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!isActive)
            return;

        float percentToUse = RollSpeedCurve.Sample(
            animHandler.CurrentFinishPercentage()
        );

        parent.Velocity =
            roll_dir * Mathf.Max(speed_buffer, MinSpeed) * percentToUse;

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

    public override void Trigger()
    {
        base.Trigger();

        if (!isActive)
            return;

        roll_dir = parent.Velocity.Normalized();
        if (Mathf.IsZeroApprox(roll_dir.LengthSquared()))
        {
            roll_dir = parent.GetComponent<Controller>().lastInput;
        }

        speed_buffer = move.currWalkSpeed;

        animHandler.PlayAnimation("roll", roll_dir);

        move.movementOverride = true;
    }

    public override void Released() { }

    public override void End()
    {
        base.End();

        StartCooldown();

        move.movementOverride = false;
        move.currWalkSpeed *= RollSpeedCurve.Sample(1.0f);
    }
}
