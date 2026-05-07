using Godot;
using Game.Entity;
using Game.Component;

public partial class CameraOverrideBoss : Area2D
{
    [Export] Node2D bossToTrack;
    [Export] float trackStrength = 0.9f;
    [Export] float trackSpeed = 0.002f;
    [Export] float zoomMin = 0.4f;
    [Export] float zoomMax = 0.6f;
    [Export] float minDistance = 0.0f;
    [Export] float maxDistance = 700.0f;

    public override void _Ready()
    {
        base._Ready();

        BodyEntered += Entered;
        BodyExited += Exited;
    }

    private void Entered(Node2D node)
    {
        if (node is not Player) return;

        var camera = (node as Player).GetComponent<OffsetCamera>();
        camera.StartNodeTrack(bossToTrack, GlobalPosition, zoomMin, zoomMax,
            minDistance, maxDistance, trackStrength);
    }

    private void Exited(Node2D node)
    {
        if (node is not Player) return;

        var camera = (node as Player).GetComponent<OffsetCamera>();
        camera.CancelNodeTrack();
    }
}
