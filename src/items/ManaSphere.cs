namespace Game.Entity;

using Godot;
using System;

public partial class ManaSphere : Area2D
{
    public override void _Ready()
    {
        base._Ready();

        // AreaEntered += ResolveCollision;
        BodyEntered += ResolveCollision;
    }

    private void ResolveCollision(Node node)
    {
        if (node is not Player)
            return;

        Player player = node as Player;

        ManaManager manaManager = player.GetComponent<ManaManager>();

        manaManager.RestoreMana(10.0f);
        
        GD.Print("AHHH!!!");

        QueueFree();
    }

}
