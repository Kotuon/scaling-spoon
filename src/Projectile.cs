using Godot;
using System;

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

        MoveAndSlide();
    }

}
