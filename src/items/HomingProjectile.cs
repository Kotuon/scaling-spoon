using Godot;
using System;
using Game.Component;

public partial class HomingProjectile : Projectile
{
    public Mouse target;
    private bool isHoming = true;
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
            tween.Finished += () =>
            {
                isHoming = false;
                launchDir = Velocity.Normalized();
            };
            hasLaunched = true;
        }
        else
        {
            var targetDir = target.mouseDir;

            // float strength = 100.0f;
            // launchDir = (launchDir + targetDir * strength).Normalized();

            float strength = 0.05f;
            if (isHoming)
            {
                var newDir =
                    (target.GlobalPosition - GlobalPosition).Normalized();

                launchDir =
                    (launchDir * (1.0f - strength) +
                    newDir * strength).Normalized();
            }

            Velocity = launchDir * currSpeed;
        }

        var collision = MoveAndCollide(Velocity * (float)delta);

        HandleCollision(collision);
    }

}
