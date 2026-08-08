namespace Game.Component;

using Game.Entity;
using Godot;

public partial class ProtectPlayer : ObstacleComponent
{
    private Player playerRef = null;

    protected override void ResolveCollisionEnter(Node node)
    {
        if (!enabled || node is not Player)
            return;

        playerRef = node as Player;
        PlayerCollider collider = playerRef.GetComponent<PlayerCollider>();
        // collider.ignoreObstacles = true;

        Global.Instance.PlayerObstacleProtection += 1;
        if (Global.Instance.PlayerObstacleProtection > 0)
        {
            collider.ignoreObstacles = true;
        }
        else
        {
            collider.ignoreObstacles = false;
        }
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (!enabled || node is not Player)
            return;

        PlayerCollider collider = playerRef.GetComponent<PlayerCollider>();
        // collider.ignoreObstacles = false;
        playerRef = null;

        Global.Instance.PlayerObstacleProtection -= 1;
        if (Global.Instance.PlayerObstacleProtection > 0)
        {
            collider.ignoreObstacles = true;
        }
        else
        {
            collider.ignoreObstacles = false;
        }
    }
}
