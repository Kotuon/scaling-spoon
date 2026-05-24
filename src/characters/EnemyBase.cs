namespace Game.Entity;

using System;
using Game.Component;
using Godot;
using Godot.Collections;

public partial class EnemyBase : CharacterBase {
    [Export]
    Array< StringName > unstunableActions = [];
    [Export]
    public Move move;
    [Export]
    public AnimationHandler animationHandler;
    [Signal]
    public delegate void HasSpawnedEventHandler();
    [Signal]
    public delegate void FinishedSpawningEventHandler();
    [Signal]
    public delegate void StartStunEventHandler();
    [Signal]
    public delegate void EndStunEventHandler();
    private AudioStreamPlayer2D _spawn_audioplayer;
    public AudioStreamPlayer2D spawn_audioplayer {
        set => _spawn_audioplayer = value;
        get {
            if ( _spawn_audioplayer == null ) {
                _spawn_audioplayer =
                    GetNode< AudioStreamPlayer2D >( "Spawn_AudioPlayer" );
            }

            return _spawn_audioplayer;
        }
    }

    private ProgressBar _healthBar = null;
    public ProgressBar healthBar {
        set {
            _healthBar = value;
            _healthBar.MaxValue = GetComponent< Health >().max;
            _healthBar.MinValue = 0.0f;
            _healthBar.Value = GetComponent< Health >().curr;
        }
        get => _healthBar;
    }

    private bool canTakeDamage = false;

    public override void _Ready() {
        base._Ready();

        AddToGroup( "Enemies", true );

        GetComponent< AnimationHandler >().PlayAnimation( "wait_to_spawn",
                                                          Vector2.Right );

        HasSpawned += PlaySpawnAudio;
        FinishedSpawning += () => { canTakeDamage = true; };

        animationHandler.animationPlayer.AnimationFinished +=
            ( StringName s ) => {
                if ( s == ( animationHandler.animationLibrary + "/hit" ) )
                    EmitSignal( SignalName.EndStun );
            };

        GetComponent< Health >().health_changed += UpdateHealth;
    }

    private void UpdateHealth( float newAmount ) {
        healthBar.Value = newAmount;
    }

    private void PlaySpawnAudio() { spawn_audioplayer.Play(); }

    public override void Damage( float amount ) {
        if ( !canTakeDamage ) return;

        base.Damage( amount );

        foreach ( var action in unstunableActions ) {
            if ( animationHandler.IsCurrentAnimationPlaying( action ) ) return;
        }

        // if ( animationHandler.IsCurrentAnimationPlaying( "hit" ) ) return;
        // if ( animationHandler.IsCurrentAnimationPlaying( "clamp" ) ) return;
        // if ( animationHandler.IsCurrentAnimationPlaying( "stomp" ) ) return;

        if ( animationHandler != null ) {
            animationHandler.PlayAnimation( "hit", Vector2.Zero );
            animationHandler.canAdvance = false;
            move.currWalkSpeed = 0.0f;
        }

        var controller = GetComponent< Controller >();
        controller.moveInput = Vector2.Zero;

        EmitSignal( SignalName.StartStun );
    }

    public override void Dies() {
        base.Dies();

        animationHandler.PlayAnimation("death", Vector2.Zero);
    }
}
