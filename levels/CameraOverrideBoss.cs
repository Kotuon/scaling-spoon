using Godot;
using Game.Entity;
using Game.Component;

public partial class CameraOverrideBoss : Area2D
{
    [Export] Node2D bossToTrack;
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
        camera.StartNodeTrack(bossToTrack, 0.4f, 0.6f, 0.0f, 700.0f, 0.75f);
    }

    private void Exited(Node2D node)
    {
        if (node is not Player) return;

        var camera = (node as Player).GetComponent<OffsetCamera>();
        camera.CancelNodeTrack();

        GD.Print("Exit");
    }
}
