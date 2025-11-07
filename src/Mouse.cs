using Godot;
using System;

public partial class Mouse : Node2D
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        var mousePos = GetGlobalMousePosition();
        mousePos -= GetViewport().GetVisibleRect().Size / 2.0f;

        // Processing if needed

        Position = mousePos;

        GD.Print("Global: " + GetGlobalMousePosition().ToString());
        GD.Print("Local: " + GetLocalMousePosition().ToString());
    }

}
