namespace Game.Entity;

using Game.Component;
using Godot;

public partial class Merchant : CharacterBase
{
    public override void _Ready()
    {
        base._Ready();

        // GetComponent<AnimationHandler>().PlayAnimation("idle", Vector2.Zero);
    }
}
