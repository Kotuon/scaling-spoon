namespace Game.Component;

using Game.Entity;
using Godot;
using System;

[Tool]
public partial class MoveToOffset : ObstacleComponent
{
    private Vector2 _targetPosition;
    public Vector2 targetPosition
    {
        set
        {
            _targetPosition = value;
        }

        get => _targetPosition;
    }
    private Vector2 startPosition;
    private float totalTime;
    [Export] private Curve tCurveStart;
    [Export] private Curve tCurveReturn;
    [Export] private float startDelay = 0.0f;

    public override void _Ready()
    {
        base._Ready();

        startPosition = GetParent<Node2D>().Position;
        targetPosition =
            (Position * parent.GlobalScale).Rotated(parent.Rotation);

        totalTime = -startDelay;
    }

    public override void _Draw()
    {
        base._Draw();

        if (Engine.IsEditorHint())
        {
            MeshInstance2D c = GetNode<MeshInstance2D>("../MeshInstance2D");
            Vector2 size = (c.Mesh as QuadMesh).Size;


            // DrawLine(GetParent<Node2D>().Position, Position,
            //     new Color(0.0f, 1.0f, 1.0f));
            DrawLine(Vector2.Zero, -Position,
                new Color(0.0f, 1.0f, 1.0f));
            DrawRect(new Rect2(-size / 2.0f, size / 1.0f),
                new Color(0.0f, 1.0f, 1.0f, 125.0f / 255.0f));
        }
    }

    public override void _Process(double delta)
    {
        base._Process(delta);

        if (!enabled) return;

        if (Engine.IsEditorHint())
        {
            QueueRedraw();
        }
        else
        {
            totalTime += (float)delta;

            float t = 0.0f;

            if (totalTime < tCurveStart.MaxDomain)
            {
                t = tCurveStart.Sample(totalTime);
            }
            else if (tCurveReturn == null)
            {
                return;
            }
            else if (totalTime < tCurveStart.MaxDomain + tCurveReturn.MaxDomain)
            {
                t = tCurveReturn.Sample(totalTime - tCurveStart.MaxDomain);
            }
            else
            {
                totalTime = 0.0f;
            }

            Vector2 lastPosition = parent.GlobalPosition;
            parent.Position = startPosition.Lerp(
                startPosition + targetPosition, t);

            parent.EmitSignal(Obstacle.SignalName.moved, lastPosition);
        }
    }
}
