using Game.Entity;
using Godot;

public partial class InactiveCollider : StaticBody2D {
    private Godot.Collections.Array< CollisionPolygon2D > colliders = [];
    // Called when the node enters the scene tree for the first time.
    public override void _Ready() {
        var children = GetChildren();

        foreach ( var child in children ) {
            if ( child is not CollisionPolygon2D ) continue;

            colliders.Add( child as CollisionPolygon2D );
        }

        // SetCollidable( false );

        var parent = GetParent< EnemyBase >();

        parent.Death += () => { SetCollidable( true ); };
        parent.HasSpawned += () => { SetCollidable( false ); };
    }

    private void SetCollidable( bool newState ) {
        foreach ( var collider in colliders ) {
            collider.Disabled = !newState;
        }
    }
}
