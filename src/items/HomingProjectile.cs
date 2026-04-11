using Godot;
using System;
using Game.Component;

public partial class HomingProjectile : Projectile
{
    public Mouse target;

    public override void _PhysicsProcess(double delta)
    {
        // if (target is null)
        // {
        //     base._PhysicsProcess(delta);
        //     return;
        // }

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
            var targetDir = target.mouseDir;

            float strength = 100.0f;
            launchDir = (launchDir + targetDir * strength).Normalized();

            Velocity = launchDir * currSpeed;
        }

        var collision = MoveAndCollide(Velocity * (float)delta);

        HandleCollision(collision);
    }

}
