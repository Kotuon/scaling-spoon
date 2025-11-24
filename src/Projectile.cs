using Godot;
using System;
using System.Threading.Tasks;

public partial class Projectile : CharacterBody2D
{
    [Export]
    public float speed = 1000.0f;
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

    public override void _Process(double delta)
    {
        base._Process(delta);

        var result = MoveAndSlide();

        if (result)
        {
            Velocity = Vector2.Zero;
            var particles = GetNode<CpuParticles2D>("HitParticles");
            particles.Emitting = true;

            var sprite = GetNode<Sprite2D>("Sprite2D");
            sprite.Visible = false;

            Timeout();
        }
    }

    public async Task Timeout()
    {
        await ToSignal(GetTree().CreateTimer(1.0f), 
            SceneTreeTimer.SignalName.Timeout);
        QueueFree();
    }

}
