namespace Game.Entity;

using Godot;
using System;

public partial class Spren : CharacterBase
{
    private AnimatedSprite2D _animPlayer;
    public AnimatedSprite2D animPlayer
    {
        private set => _animPlayer = value;

        get
        {
            if (_animPlayer == null)
                _animPlayer = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
            return _animPlayer;
        }
    }
    public override void _Ready()
    {
        base._Ready();

        animPlayer.Play("default");
        // animPlayer.Play("Spren/idle");
    }

}
