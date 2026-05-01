namespace Game.Component;

using Game.Entity;
using Godot;
using System;

public partial class OffsetCamera : Camera2D
{

    private Node2D targetToTrack = null;
    private Vector2 targetOffset = Vector2.Zero;
    private Vector2 targetZoom;
    private float targetSpeed = 0.125f;
    private float trackStr = 0.5f;

    private Player _parent;

    protected Player parent
    {
        private set => _parent = value;

        get
        {
            if (_parent == null)
                _parent = GetParent() as Player;
            return _parent;
        }
    }

    private Vector2 defaultZoom;

    private bool useOffset = false;

    public partial class ZoomValues
    {
        public ZoomValues(float z_min, float z_max, float d_min, float d_max)
        {
            zoom_min = z_min;
            zoom_max = z_max;
            dist_min = d_min;
            dist_max = d_max;
        }
        public float zoom_min;
        public float zoom_max;
        public float dist_min;
        public float dist_max;

        public float get_new_zoom(float curr_dist)
        {
            return Mathf.Lerp(zoom_min, zoom_max,
                1.0f - Mathf.Clamp(curr_dist, dist_min, dist_max) / dist_max);
        }
    }

    private ZoomValues zoom_values;

    public override void _Ready()
    {
        base._Ready();

        defaultZoom = Zoom;
        targetZoom = defaultZoom;
    }


    public void StartNodeTrack(Node2D target, float zoom_min, float zoom_max,
        float dist_min, float dist_max, float str = 0.5f, float speed = 0.0075f)
    {
        targetToTrack = target;
        targetSpeed = speed;

        zoom_values = new ZoomValues(zoom_min, zoom_max, dist_min, dist_max);

        trackStr = str;
    }

    public void CancelNodeTrack()
    {
        targetToTrack = null;
        targetOffset = Vector2.Zero;
        targetZoom = defaultZoom;
    }

    public void TriggerOffset(Vector2 target, float speed = 0.125f)
    {
        if (targetToTrack != null) return;

        targetOffset = target;
        targetSpeed = speed;
    }

    public void CancelOffset()
    {
        targetOffset = Vector2.Zero;
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        Update();
    }

    private void Update()
    {
        if (targetToTrack != null)
        {
            var between = targetToTrack.GlobalPosition - parent.GlobalPosition;

            float dist = between.Length();
            GD.Print(dist);

            targetOffset = between.Normalized() * dist * trackStr;

            float new_zoom = zoom_values.get_new_zoom(dist);
            targetZoom = new Vector2(new_zoom, new_zoom);

            // float new_zoom =

            // targetOffset =
            //     between * between.Length() / 2.0f;
        }
        Offset = Offset.Lerp(targetOffset, targetSpeed);
        Zoom = Zoom.Lerp(targetZoom, targetSpeed);
        QueueRedraw();

    }

    public override void _Draw()
    {
        base._Draw();

        // target
        DrawCircle(targetOffset, 10.0f, new Color(1, 0, 0, .5f));

        // Player
        DrawCircle(ToLocal(parent.GlobalPosition), 5.0f, new Color(0, 0, 1, .5f));

        if (targetToTrack != null)
            DrawCircle(ToLocal(targetToTrack.GlobalPosition), 20.0f, new Color(0, 1, 0, .5f));
    }

}
