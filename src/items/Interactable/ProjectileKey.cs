namespace Game.Entity;

using Godot;

public partial class ProjectileKey : Key
{
    protected override void ResolveCollisionEnter(Node node)
    {
        base.ResolveCollisionEnter(node);

        if (node is Projectile && (node as Projectile).owner is Player) {
            completed = true;
        }
    }
}
