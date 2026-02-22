namespace Game.Entity;

using Godot;
using System;

public partial class Spren : CharacterBase
{
    private AnimationPlayer _animPlayer;
    public AnimationPlayer animPlayer
    {
        private set => _animPlayer = value;

        get
        {
            if (_animPlayer == null)
                _animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
            return _animPlayer;
        }
    }
    public override void _Ready()
    {
        base._Ready();

        animPlayer.Play("Spren/idle");
    }

}
