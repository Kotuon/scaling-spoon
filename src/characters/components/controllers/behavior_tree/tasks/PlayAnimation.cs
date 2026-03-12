namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

public partial class PlayAnimation : BehaviorNode
{
    [Export] string animationName;
    public override BehaviorNode.Status evaluate(Dictionary context)
    {
        var parent = context["parent"].As<CharacterBase>();
        if (parent == null)
        {
            GD.PushError("No parent.");
            return BehaviorNode.Status.ERROR;
        }

        var animationHandler = parent.GetComponent<AnimationHandler>();
        if (animationHandler == null)
        {
            GD.PushError("No animation handler.");
            return BehaviorNode.Status.ERROR;
        }

        var move = parent.GetComponent<Move>();
        if (move != null)
            move.canMove = false;

        animationHandler.PlayAnimation(animationName, Vector2.Zero);

        return BehaviorNode.Status.SUCCESS;
    }
}
