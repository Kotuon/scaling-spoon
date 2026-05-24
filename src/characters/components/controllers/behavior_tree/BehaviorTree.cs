namespace Game.Component;

using Game.Entity;
using Godot;
using Godot.Collections;

public partial class BehaviorTree : BehaviorNode {
    BehaviorNode root;

    private Dictionary m_context = [];

    public override void _Ready() {
        base._Ready();

        root = ( BehaviorNode )GetChild( 0 );

        m_context.Add( "parent", GetParent() as CharacterBase );
        m_context.Add( "blocked", false );

        var parent = GetParent() as EnemyBase;

        parent.StartStun += () => { m_context["blocked"] = true; };

        parent.EndStun += () => { m_context["blocked"] = false; };

        parent.Death += () => { m_context["blocked"] = true; GD.Print("Died"); };
    }

    public override void _Process( double delta ) {
        if ( ( bool )m_context["blocked"] ) return;

        base._Process( delta );

        m_context["delta"] = delta;
        root.evaluate( m_context );
    }
}
