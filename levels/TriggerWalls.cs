using Game.Entity;
using Godot;

public partial class TriggerWalls : StaticBody2D {
    Godot.Collections.Array< CollisionShape2D > walls = [];
    Godot.Collections.Array< Node2D > pillars = [];
    public override void _Ready() {
        base._Ready();

        var boss = GetNode< EnemyBase >( "../Golem" );
        boss.HasSpawned += OnTrigger;
        boss.Death += OnBossDeath;

        var children = GetChildren();
        foreach ( var child in children ) {
            if ( child is CollisionShape2D ) {
                walls.Add( child as CollisionShape2D );
                ( child as CanvasItem ).Visible = false;
            } else if ( child.Name.ToString().Contains( "Pillar" ) ) {
                pillars.Add( child as Node2D );

                child.GetNode< Sprite2D >( "PillarSprite" ).Visible = false;

                var effect = child.GetNode< Sprite2D >( "Effect" );
                effect.Frame = effect.Hframes - 1;

                var collider = child.GetNode< CollisionShape2D >( "Collider" );
                collider.Disabled = true;
            }
        }

        SetCollidable( false );
    }

    private void OnTrigger() {
        // TODO: Make walls solid
        SetCollidable( true );
        PlayPillarAnim();
    }

    private void OnBossDeath() { SetCollidable( false ); }

    private void SetCollidable( bool collidable ) {
        foreach ( var wall in walls ) {
            wall.Disabled = !collidable;
            wall.Visible = collidable;
        }
    }

    private void PlayPillarAnim() {
        foreach ( var pillar in pillars ) {
            var effectPlayer =
                pillar.GetNode< AnimationPlayer >( "Effect/AnimationPlayer" );
            effectPlayer.Play( "base" );

            var collider = pillar.GetNode< CollisionShape2D >( "Collider" );
            collider.Disabled = false;
        }
    }
}
