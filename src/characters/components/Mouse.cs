
namespace Game.Component;

using Godot;
using Game.Math;

public partial class Mouse : Component
{

    [Export] public float offsetPercent = 0.25f;
    [Export] public float maxRadius = 250.0f;

    private bool _useMouseDirection;

    public bool useMouseDirection
    {
        set
        {
            if (value)
            {
                icon.Visible = true;

                Vector2 newPosition = GetViewportRect().Size / 2.0f;

                Input.WarpMouse(newPosition + new Vector2(0.0f, 20.0f));
            }
            else
            {
                icon.Visible = false;

                var camera = parent.GetComponent<OffsetCamera>();
                if (camera != null)
                    camera.CancelOffset();
            }

            _useMouseDirection = value;
        }

        get => _useMouseDirection;
    }

    private Vector2 _mouseDir;
    public Vector2 mouseDir
    {
        private set => _mouseDir = value;
        get
        {
            _mouseDir = GlobalPosition - parent.GlobalPosition;
            return _mouseDir.Normalized();
        }
    }

    protected MeshInstance2D icon;

    public override void _Ready()
    {
        base._Ready();

        foreach (var child in GetChildren())
        {
            if (child is MeshInstance2D)
                icon = (MeshInstance2D)child;
        }

        Visible = true;
        icon.Visible = false;
    }
    public override void _Process(double delta)
    {
        base._Process(delta);

        var joystick = Input.GetVector("mouse_left", "mouse_right", "mouse_up",
            "mouse_down");

        joystick = Curves.CubicBezier(Vector2.Zero, Vector2.Zero, joystick * 0.25f,
            joystick, joystick.Length());

        joystick *= maxRadius;

        var mousePos = GetGlobalMousePosition() + joystick;

        var dir = mousePos - parent.GlobalPosition;
        if (dir.Length() > maxRadius)
            dir = dir.Normalized() * maxRadius;

        mousePos = parent.GlobalPosition + dir;

        // Processing if needed

        GlobalPosition = mousePos;

        if (useMouseDirection)
        {
            parent.GetComponent<OffsetCamera>().TriggerOffset(
                (GlobalPosition - parent.GlobalPosition) * offsetPercent
            );
        }
    }


}
