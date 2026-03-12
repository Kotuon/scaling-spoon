namespace Game.Entity;

using Game.Component;
using Godot;
using System;

public partial class EnemyBase : CharacterBase
{
    public override void _Ready()
    {
        base._Ready();

        GetComponent<AnimationHandler>().PlayAnimation(
            "wait_to_spawn", Vector2.Right);
    }
}
