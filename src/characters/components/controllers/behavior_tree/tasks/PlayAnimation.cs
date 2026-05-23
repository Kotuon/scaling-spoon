namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

public partial class PlayAnimation : BehaviorNode {
    [Export]
    string animationName;

    private bool animStarted = false;
    public override BehaviorNode.Status evaluate( Dictionary context ) {
        // Verifying parent exists
        var parent = context["parent"].As< CharacterBase >();
        if ( parent == null ) {
            GD.PushError( "No parent." );
            return BehaviorNode.Status.ERROR;
        }

        // Verifying animation handler
        var animHandler = parent.GetComponent< AnimationHandler >();
        if ( animHandler == null ) {
            GD.PushError( "No animation handler." );
            return BehaviorNode.Status.ERROR;
        }

        // Animation finished
        if ( animStarted && !animHandler.IsCurrentlyPlaying() ) {
            AnimFinished( context, parent );
            return BehaviorNode.Status.SUCCESS;
        }

        if ( /* Animation needs to start checks */ !animStarted ) {
            AnimStarted( context, parent, animHandler );
        }

        return BehaviorNode.Status.RUNNING;
    }

    private void AnimFinished( Dictionary context, CharacterBase parent ) {
        // context["blocked"] = false;
        animStarted = false;

        var move = parent.GetComponent< Move >();
        if ( move != null ) move.canMove = true;

        // GD.Print( "Animation Finished: " + animationName );
    }

    private void AnimStarted( Dictionary context, CharacterBase parent,
                              AnimationHandler animHandler ) {
        animHandler.PlayAnimation( animationName, Vector2.Zero );
        // context["blocked"] = true;

        animStarted = true;

        var move = parent.GetComponent< Move >();
        if ( move != null ) move.canMove = false;

        // GD.Print( "Animation Started: " + animationName );
    }
}
