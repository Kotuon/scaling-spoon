using Godot;
using System;

public partial class MainUI : Control
{
    private Camera2D _camera;
    public Camera2D camera
    {
        private set => _camera = value;

        get
        {
            if (_camera == null)
                _camera = GetNode<Camera2D>("..");
            return _camera;
        }
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        Position = camera.Position + camera.Offset;
    }

}
