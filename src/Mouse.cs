
namespace Game.Component;

using Godot;
using System;

public partial class Mouse : Component
{

    [Export]
    public float maxRadius = 400.0f;

    private bool _useMouseDirection;

    public bool useMouseDirection
    {
        set
        {
            if (value)
            {
                icon.Visible = true;
                Input.WarpMouse(GetViewportRect().Size / 2.0f);
            }
            else
            {
                icon.Visible = false;
            }

            _useMouseDirection = value;
        }

        get => _useMouseDirection;
    }

    private MeshInstance2D icon;

    public override void _Ready()
    {
        base._Ready();

        foreach (var child in GetChildren())
        {
            if (child is MeshInstance2D)
                icon = (MeshInstance2D)child;
        }

        icon.Visible = false;

        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
    }


    public override void _Process(double delta)
    {
        base._Process(delta);

        var mousePos = GetGlobalMousePosition();

        var dir = mousePos - parent.GlobalPosition;

        if (dir.Length() > maxRadius)
        {
            mousePos = parent.GlobalPosition + dir.Normalized() * maxRadius;
        }

            // Processing if needed

            GlobalPosition = mousePos;
    }

}
