namespace Game.Component;

using System;
using Game.Entity;
using Godot;

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

    public override void _Draw()
    {
        base._Draw();

        DrawLine(Vector2.Zero, moveInput * 100.0f, new Color(0.0f, 0.0f, 1.0f));
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        QueueRedraw();
    }
}
