namespace Game.Entity;

using Godot;

public partial class Obstacle : AreaTriggerItem
{
    [Signal] public delegate void movedEventHandler(Vector2 lastPosition);
    [Signal] public delegate void effectPlayerEventHandler(Player player);

    public virtual void EffectPlayer(Player playerRef)
    {
        EmitSignal(SignalName.effectPlayer, playerRef);
    }
}
