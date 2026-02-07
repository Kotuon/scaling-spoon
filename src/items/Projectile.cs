using Game.Entity;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Projectile : CharacterBody2D
{
    [Export] public float initialSpeed = 1000.0f;
    [Export] public float finalSpeed = 1000.0f;
    [Export] public float timeToAccelerate = 0.0f;
    public GodotObject owner;
    private Vector2 _launchDir;
    public Vector2 launchDir
    {
        set
        {
            _launchDir = value;
            currSpeed = initialSpeed;
            // Velocity = _launchDir * initialSpeed;
        }

        get => _launchDir;
    }

    private float currSpeed;
    private bool hasLaunched = false;

    public float damage = 1.0f;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        if (!hasLaunched)
        {
            Tween tween = GetTree().CreateTween();
            tween.TweenProperty(
                this, "currSpeed", finalSpeed, timeToAccelerate
            );
            hasLaunched = true;
        }
        else
        {
            Velocity = launchDir * currSpeed;
        }

        var collision = MoveAndCollide(Velocity * (float)delta);

        if (collision != null && collision.GetCollider() != owner)
        {
            DestroyProjectile();

            if (collision.GetCollider() is IDamageable)
            {
                // call damage function
                (collision.GetCollider() as IDamageable).Damage(damage);
            }

            _ = Timeout();
        }
    }

    private void DestroyProjectile()
    {
        Velocity = Vector2.Zero;
        var particles = GetNode<CpuParticles2D>("HitParticles");
        particles.Emitting = true;

        var sprite = GetNode<Sprite2D>("Sprite2D");
        sprite.Visible = false;

        var collider = GetNode<CollisionShape2D>("Hitbox");
        collider.Disabled = true;
    }

    public async Task Timeout()
    {
        await ToSignal(GetTree().CreateTimer(1.0f),
            SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }

}
