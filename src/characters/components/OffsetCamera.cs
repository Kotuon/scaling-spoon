namespace Game.Component;

using System;
using System.Collections.Generic;
using System.Net;
using Game.Entity;
using Godot;

public partial class OffsetCamera : Camera2D {
    [Export]
    private FastNoiseLite shake_noise;
    [Export( PropertyHint.Range, "0, 1," )]
    private float decay = 0.8f;
    [Export( PropertyHint.Range, "2, 3," )]
    private float trauma_strength = 2.0f;
    [Export]
    private Vector2 max_shake_offset = new( 100.0f, 75.0f );
    [Export]
    private float max_roll = 0.0f;
    private float trauma;
    private float noise_y = 0.0f;
    private Vector2 shake_offset = Vector2.Zero;

    private Node2D targetToTrack = null;
    private Vector2 targetOrigin = Vector2.Zero;
    private Vector2 targetOffset = Vector2.Zero;
    private Vector2 targetZoom;
    private float targetSpeed = 1.0f; // 0.125f
    private float trackStr = 0.5f;

    private Player _parent;

    protected Player parent { private
        set => _parent = value;

        get {
            _parent ??= GetParent() as Player;
            return _parent;
        }
    }

    private Vector2 defaultZoom;
    [Export]
    private bool debug_draw = true;

    public partial class ZoomValues( float z_min, float z_max, float d_min,
                                     float d_max ) {
        public float zoom_min = z_min;
        public float zoom_max = z_max;
        public float dist_min = d_min;
        public float dist_max = d_max;

        public float get_new_zoom( float curr_dist ) {
            return Mathf.Lerp(
                zoom_min, zoom_max,
                1.0f -
                    Mathf.Clamp( curr_dist, dist_min, dist_max ) / dist_max );
        }
    }

    private ZoomValues zoom_values;

    // private bool running_tween = false;
    private Dictionary<String, bool> running_tween = [];
    private bool has_locked = false;

    public override void _Ready() {
        base._Ready();

        defaultZoom = Zoom;
        targetZoom = defaultZoom;

        running_tween.Add("targetOrigin", false);
        running_tween.Add("targetOffset", false);
    }

    public void StartNodeTrack( Node2D target, Vector2 lock_pos, float zoom_min,
                                float zoom_max, float dist_min, float dist_max,
                                float str = 0.5f, float speed = 0.004f ) {
        targetToTrack = target;
        targetSpeed = speed;

        zoom_values = new ZoomValues( zoom_min, zoom_max, dist_min, dist_max );

        trackStr = str;

        has_locked = false;
        targetOffset = Vector2.Zero;
    }

    public void CancelNodeTrack() {
        targetToTrack = null;

        // targetOffset += targetOrigin - parent.GlobalPosition;
        targetOffset = GlobalPosition - parent.GlobalPosition;
        // tween_vector2( "targetOffset", Vector2.Zero, 5.0f, true );
        // tween_vector2("targetOrigin", parent.GlobalPosition, 0.0f, true);
        // targetOrigin = parent.GlobalPosition;

        targetZoom = defaultZoom;
    }

    public void TriggerOffset( Vector2 target, float timeToOffset = 0.25f ) {
        if ( targetToTrack != null ) return;

        tween_vector2( "targetOffset", target, timeToOffset, true );
        targetSpeed = timeToOffset;
    }

    public void CancelOffset() {
        tween_vector2( "targetOffset", Vector2.Zero, override_curr: true );
        targetSpeed = 0.125f;
    }

    public override void _Process( double delta ) {
        base._Process( delta );

        Update();
    }

    private void Update() {
        if ( targetToTrack != null ) {
            var avg = targetToTrack.GlobalPosition * trackStr +
                      parent.GlobalPosition * ( 1.0f - trackStr );

            var between = targetToTrack.GlobalPosition - parent.GlobalPosition;
            float dist = between.Length();

            float speed = 0.25f;
            bool can_override = true;
            if ( !has_locked ) {
                speed = 2.0f;
                can_override = false;

                var timer = GetTree().CreateTimer( speed );
                timer.Timeout += () => has_locked = true;
            }

            tween_vector2( "targetOrigin", avg, speed, can_override );

            float new_zoom = zoom_values.get_new_zoom( dist );
            targetZoom = new Vector2( new_zoom, new_zoom );
        } else {
            targetOrigin = parent.GlobalPosition;
        }

        // if (trauma > 0.0)
        // {
        //     trauma = Mathf.Max(trauma - decay, 0.0f);
        //     shake();
        //     Offset = shake_offset;
        // }
        // else
        // {
        //     Position = Position.Lerp(Vector2.Zero, targetSpeed);
        // }

        GlobalPosition = targetOrigin + targetOffset;

        Zoom = Zoom.Lerp( targetZoom, targetSpeed );
        QueueRedraw();
    }

    public void add_trauma( float amount ) {
        trauma = Mathf.Min( trauma + amount, 1.0f );
    }

    private void shake() {
        var amount = Mathf.Pow( trauma, trauma_strength );
        noise_y += 1.0f;
        Rotation = max_roll * amount *
                   shake_noise.GetNoise2D( shake_noise.Seed, noise_y );
        shake_offset.X =
            max_shake_offset.X * amount *
            shake_noise.GetNoise2D( shake_noise.Seed * 2, noise_y );
        shake_offset.Y =
            max_shake_offset.Y * amount *
            shake_noise.GetNoise2D( shake_noise.Seed * 3, noise_y );
    }

    public override void _Draw() {
        if ( !debug_draw ) return;

        base._Draw();

        // target
        DrawCircle( ToLocal( targetOrigin + targetOffset ), 10.0f,
                    new Color( 1, 0, 0, .5f ) );
        DrawCircle( ToLocal( GlobalPosition ), 30.0f,
                    new Color( 1, 1, 0, .5f ) );

        // Player
        DrawCircle( ToLocal( parent.GlobalPosition ), 5.0f,
                    new Color( 0, 0, 1, .5f ) );

        if ( targetToTrack != null )
            DrawCircle( ToLocal( targetToTrack.GlobalPosition ), 20.0f,
                        new Color( 0, 1, 0, .5f ) );
    }

    private void tween_vector2( string vec_name, Vector2 target,
                                float time = 0.25f,
                                bool override_curr = false ) {
        if ( running_tween[vec_name] && !override_curr ) return;

        running_tween[vec_name] = true;

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty( this, vec_name, target, time );

        tween.Finished += () => running_tween[vec_name] = false;
    }
}
