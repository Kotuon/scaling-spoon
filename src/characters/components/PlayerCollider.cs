namespace Game.Component;

using Godot;
using Game.Entity;

public partial class PlayerCollider : CollisionShape2D
{
    private bool _ignoreObstacles;
    public bool ignoreObstacles
    {
        set
        {
            _ignoreObstacles = value;
            if (!ignoreObstacles && inObstacle)
            {
                // Trigger collision
                obstacleRef.EffectPlayer(GetParent() as Player);
            }
        }

        get => _ignoreObstacles;
    }
    private bool inObstacle = false;

    private Obstacle obstacleRef;

    public override void _Ready()
    {
        base._Ready();

        ignoreObstacles = false;
        inObstacle = false;
    }

    public void SetObstacle(Obstacle obstacle, bool state)
    {
        obstacleRef = obstacle;
        inObstacle = state;
    }
}
