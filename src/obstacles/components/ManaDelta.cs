namespace Game.Component;

using Godot;
using Game.Entity;

public partial class ManaDelta : ObstacleComponent
{
    [Export] float deltaAmount = 10.0f;
    private Player playerRef = null;
    private bool inside = false;

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!inside) return;

        Mana manaManager = playerRef.GetComponent<Mana>();
        manaManager.ChangeMana(deltaAmount * (float)delta);
    }

    protected override void ResolveCollisionEnter(Node node)
    {
        if (!enabled || node is not Player) return;

        inside = true;
        playerRef = node as Player;
    }

    protected override void ResolveCollisionExit(Node node)
    {
        if (!enabled || node is not Player) return;

        inside = false;
        playerRef = null;
    }
}
