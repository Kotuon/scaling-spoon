using Godot;
using Game.Entity;
using Game.Component;
using System.Reflection.Metadata;

public partial class CameraOverrideBoss : Area2D {
    [Export]
    Node2D bossToTrack;
    [Export]
    float trackStrength = 0.9f;
    [Export]
    float trackSpeed = 0.002f;
    [Export]
    float zoomMin = 0.4f;
    [Export]
    float zoomMax = 0.6f;
    [Export]
    float minDistance = 0.0f;
    [Export]
    float maxDistance = 700.0f;

    private Player playerRef = null;
    private bool ignore = false;

    public override void _Ready() {
        base._Ready();

        BodyEntered += Entered;
        BodyExited += Exited;

        ( bossToTrack as CharacterBase ).Death += async () => {
            await ToSignal( GetTree().CreateTimer( 2.0 ),
                            SceneTreeTimer.SignalName.Timeout );

            ignore = true;

            if ( playerRef == null ) return;

            var camera = playerRef.GetComponent< OffsetCamera >();
            camera.CancelNodeTrack();

            var boss = bossToTrack as EnemyBase;

            boss.healthBar.QueueFree();
        };
    }

    private void Entered( Node2D node ) {
        if ( ignore || node is not Player ) return;
        playerRef = ( node as Player );

        var camera = ( node as Player ).GetComponent< OffsetCamera >();
        camera.StartNodeTrack( bossToTrack, GlobalPosition, zoomMin, zoomMax,
                               minDistance, maxDistance, trackStrength );

        var boss = ( bossToTrack as EnemyBase );

        boss.healthBar =
            ( ProgressBar )GD
                .Load< PackedScene >( "res://menus/HealthBar.tscn" )
                .Instantiate();

        ( node as Player )
            .GetNode( "Camera2D/Control/Locked" )
            .AddChild( boss.healthBar );
    }

    private void Exited( Node2D node ) {
        if ( ignore || node is not Player ) return;

        var camera = ( node as Player ).GetComponent< OffsetCamera >();
        camera.CancelNodeTrack();
    }
}
