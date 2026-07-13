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
    private bool returnPass = false;
    [Export] private Curve tCurveStart;
    [Export] private Curve tCurveReturn;
    [Export] private float startDelay = 0.0f;
    [Export] private bool runOnce = false;
    [Export] private bool needTriggerEachRun = false;

    public override void _Ready()
    {
        if (!Engine.IsEditorHint())
        {
            base._Ready();

            if (tCurveReturn == null)
            {
                tCurveReturn = tCurveStart.Duplicate() as Curve;

                int count = tCurveReturn.PointCount;
                for (int i = 0; i < count; ++i)
                {
                    var pos = tCurveStart.GetPointPosition(count - i - 1);
                    tCurveReturn.SetPointOffset(i, pos.X);
                    tCurveReturn.SetPointValue(i, pos.Y);
                }
            }

            var tpar = GetParent<Node2D>();

            startPosition = GetParent<Node2D>().Position;
            targetPosition =
                (Position / tpar.GetParent<Node2D>().GlobalScale * GlobalScale).Rotated(tpar.Rotation);

            GD.Print(tpar.GlobalScale);
            GD.Print(startPosition);
            GD.Print(targetPosition);

            totalTime = -startDelay;
        }
    }

    public override void _Draw()
    {
        base._Draw();

        if (Engine.IsEditorHint())
        {
            if (GetParent() is not Area2D) return;
            MeshInstance2D c = GetNode<MeshInstance2D>("../MeshInstance2D");
            Vector2 size = (c.Mesh as QuadMesh).Size;

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
            else if (runOnce || (needTriggerEachRun && !returnPass)/* tCurveReturn == null */)
            {
                ResetKeys();
                enabled = false;
                returnPass = true;
                return;
            }
            else if (totalTime < tCurveStart.MaxDomain + tCurveReturn.MaxDomain)
            {
                t = tCurveReturn.Sample(totalTime - tCurveStart.MaxDomain);
            }
            else
            {
                totalTime = 0.0f;

                ResetKeys();
                enabled = false;
                returnPass = false;
            }

            Vector2 lastPosition = parent.Position;
            parent.Position = startPosition.Lerp(
                startPosition + targetPosition, t);

            parent.EmitSignal(Obstacle.SignalName.moved, lastPosition);
        }
    }
}
