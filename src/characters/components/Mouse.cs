
namespace Game.Component;

using Godot;
// using System.Numerics;


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

                parent.GetComponent<OffsetCamera>().CancelOffset();
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

    private MeshInstance2D icon;

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

        Input.MouseMode = Input.MouseModeEnum.ConfinedHidden;
    }

    private float QuadLerp(float a, float b, float c, float t)
    {
        float p0 = Mathf.Lerp(a, b, t);
        float p1 = Mathf.Lerp(b, c, t);
        float p2 = Mathf.Lerp(p0, p1, t);

        return p2;
    }
    private float CubicLerp(float a, float b, float c, float d, float t)
    {
        float p0 = Mathf.Lerp(a, b, t);
        float p1 = Mathf.Lerp(b, c, t);
        float p2 = Mathf.Lerp(c, d, t);

        float p3 = Mathf.Lerp(p0, p1, t);
        float p4 = Mathf.Lerp(p1, p2, t);

        float p5 = Mathf.Lerp(p3, p4, t);

        return p5;
    }
    private Godot.Vector2 QuadraticBezier(Godot.Vector2 p0, Godot.Vector2 p1, Godot.Vector2 p2, float t)
    {
        Godot.Vector2 q0 = p0.Lerp(p1, t);
        Godot.Vector2 q1 = p1.Lerp(p2, t);

        return q0.Lerp(q1, t);
    }
    private Vector2 CubicBezier(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3, float t)
{
    Vector2 q0 = p0.Lerp(p1, t);
    Vector2 q1 = p1.Lerp(p2, t);
    Vector2 q2 = p2.Lerp(p3, t);

    Vector2 r0 = q0.Lerp(q1, t);
    Vector2 r1 = q1.Lerp(q2, t);

    Vector2 s = r0.Lerp(r1, t);
    return s;
}
    public override void _Process(double delta)
    {
        base._Process(delta);

        // var joystick = parent.GetComponent<Controller>().moveInput;
        var joystick = Input.GetVector("mouse_left", "mouse_right", "mouse_up",
            "mouse_down");

        // joystick.X = CubicLerp(0.0f, 0.0f, 0.0f, 1.0f, Mathf.Abs(joystick.X)) * (joystick.X);
        // joystick.Y = CubicLerp(0.0f, 0.0f, 0.0f, 1.0f, Mathf.Abs(joystick.Y)) * (joystick.Y);

        // joystick = QuadraticBezier(Godot.Vector2.Zero, joystick * 0.25f,
        //     joystick, joystick.Length());

        joystick = CubicBezier(Vector2.Zero, Vector2.Zero, joystick * 0.25f,
            joystick, joystick.Length());

        GD.Print(joystick);

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
