using Game.Entity;
using Godot;
using System;
using System.Threading.Tasks;

public partial class Projectile : CharacterBody2D
{
    [Export]
    public float speed = 1000.0f;
    public GodotObject owner;
    private Vector2 _launchDir;
    public Vector2 launchDir
    {
        set
        {
            _launchDir = value;
            Velocity = _launchDir * speed;
        }

        get => _launchDir;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);

        // var result = MoveAndSlide();
        var collision = MoveAndCollide(Velocity * (float)delta);

        if (collision != null && collision.GetCollider() != owner)
        {
            DestroyProjectile();

            if (collision.GetCollider() is IDamageable)
            {
                // call damage function
                (collision.GetCollider() as IDamageable).Damage(0.0f);
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
