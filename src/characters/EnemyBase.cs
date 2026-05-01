namespace Game.Entity;

using Game.Component;
using Godot;
using System;

public partial class EnemyBase : CharacterBase
{
    [Signal] public delegate void HasSpawnedEventHandler();
    private AudioStreamPlayer2D _spawn_audioplayer;
    public AudioStreamPlayer2D spawn_audioplayer
    {
        set => _spawn_audioplayer = value;
        get
        {
            if (_spawn_audioplayer == null)
            {
                _spawn_audioplayer =
                    GetNode<AudioStreamPlayer2D>("Spawn_AudioPlayer");
            }

            return _spawn_audioplayer;
        }
    }
    public override void _Ready()
    {
        base._Ready();

        AddToGroup("Enemies", true);

        GetComponent<AnimationHandler>().PlayAnimation(
            "wait_to_spawn", Vector2.Right);

        HasSpawned += PlaySpawnAudio;
    }

    private void PlaySpawnAudio()
    {
        spawn_audioplayer.Play();
    }
}
