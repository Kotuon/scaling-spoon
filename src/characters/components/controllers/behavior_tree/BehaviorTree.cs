namespace Game.Component;

using Game.Entity;
using Godot.Collections;

public partial class BehaviorTree : BehaviorNode {
    BehaviorNode root;

    private Dictionary m_context = [];

    public override void _Ready() {
        base._Ready();

        root = ( BehaviorNode )GetChild( 0 );

        m_context.Add( "parent", GetParent() as CharacterBase );
        m_context.Add( "blocked", false );
        ( GetParent() as EnemyBase ).StartStun +=
            () => { m_context["blocked"] = true; };

        ( GetParent() as EnemyBase ).EndStun +=
            () => { m_context["blocked"] = false; };
    }

    public override void _Process( double delta ) {
        if ( ( bool )m_context["blocked"] ) return;

        base._Process( delta );

        m_context["delta"] = delta;
        root.evaluate( m_context );
    }
}
