using Game.Entity;
using Godot;

public partial class TriggerWalls : StaticBody2D
{
    Godot.Collections.Array<CollisionShape2D> walls =
        new Godot.Collections.Array<CollisionShape2D>();
    public override void _Ready()
    {
        base._Ready();

        var boss = GetNode<EnemyBase>("../YSort/Golem");
        boss.HasSpawned += OnTrigger;

        var children = GetChildren();
        foreach (var child in children)
        {
            walls.Add(child as CollisionShape2D);
        }

        SetCollidable(false);
    }

    private void OnTrigger()
    {
        // TODO: Make walls solid
        SetCollidable(true);
    }

    private void SetCollidable(bool collidable)
    {
        foreach (var wall in walls)
        {
            wall.Disabled = !collidable;
        }
    }
}
