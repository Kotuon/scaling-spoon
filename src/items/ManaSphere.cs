namespace Game.Entity;

using Godot;
using System;

public partial class ManaSphere : AreaTriggerItem
{
    [Export] float restore_amount = 10.0f;
    protected override void ResolveCollisionEnter(Node node)
    {
        if (node is not Player)
            return;

        Player player = node as Player;

        Mana manaManager = player.GetComponent<Mana>();
        manaManager.RestoreMana(10.0f);

        QueueFree();
    }

}
