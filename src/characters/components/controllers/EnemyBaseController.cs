namespace Game.Component;

using Game.Entity;
using Godot;
using System;

public partial class EnemyBaseController : Controller
{
    private CharacterBase _player = null;
    protected CharacterBase player
    {
        private set => _player = value;

        get
        {
            if (_player == null)
            {
                Node root = GetTree().Root.GetChild(0);

                _player = root.FindChild("Player") as Player;
            }

            return _player;
        }
    }
    public override void _Ready()
    {
        base._Ready();
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        // if (parent.Position.DistanceTo(player.Position) > 200.0f)
        //     moveInput = (player.Position - parent.Position).Normalized();
        // else
        //     moveInput = Vector2.Zero;
    }

}
